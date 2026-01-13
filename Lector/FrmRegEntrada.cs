using DPFP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.IO;



namespace Lector
{
    public partial class FrmRegEntrada : Form, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture capturer;
        private DPFP.FeatureSet featuresActuales;
        private List<string> memberCodes = new List<string>();


        private void FrmRegEntrada_FormClosed(object sender, FormClosedEventArgs e)
        {
            LimpiarLector();
        }
        private void LimpiarLector()
        {
            if (capturer != null)
            {
                try
                {
                    capturer.StopCapture();
                }
                catch { }

                capturer.EventHandler = null;
                capturer = null;
            }

            featuresActuales = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        public FrmRegEntrada()
        {
            InitializeComponent();
            this.FormClosed += FrmRegEntrada_FormClosed;
            IniciarCaptura(); // COLOCAR AQUI PARA QUE COMIENCE LA CAPTURA TAL CUAL EMPIEZA

        }

        private void IniciarCaptura()
        {
            try
            {
                capturer = new DPFP.Capture.Capture();
                capturer.EventHandler = this;
                capturer.StartCapture();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar lector: " + ex.Message);
            }
        }

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            

            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (this.IsDisposed) return;
                    MessageBox.Show("Huella Capturada espere");
                    featuresActuales = ExtraerFeaturesVerificacion(Sample);

                    pictureBox1.Image = new Bitmap(
                        ConvertSampleToBitmap(Sample),
                        pictureBox1.Size
                    );
                    int index = CompararHuellaEnBD(featuresActuales);
                    if (index >= 0)
                    {
                        string memberCode = memberCodes[index];
                        MessageBox.Show("ACCESO CONCEDIDO\nMember Code: " + memberCode);
                    }
                    else
                    {
                        MessageBox.Show("NO RECONOCIDO");
                    }



                }));
            }
            catch
            {
             
            }
        }
        private int CompararHuellaEnBD(DPFP.FeatureSet features)
        {
            if (features == null) return -1;
            

            List<string> fingerprints = new List<string>();

            string connectionString =
            "Host=TUHOST;" +
            "Port=TUPUERTO;" +
            "Database=TUBASEDEDATOS;" +
            "Username=TUNOMBREDEUSUARIO;" +
            "Password=TUCONTRASEÑA;" +
            "SslMode=Require;" +
            "Trust Server Certificate=true;";

            //  Cargar huellas desde BD
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "SELECT fingerprint_data, member_code FROM customers WHERE fingerprint_data IS NOT NULL",
                    conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fingerprints.Add(reader.GetString(0));
                        memberCodes.Add(reader.GetString(1));
                    }
                }
            }
            int index = BuscarCoincidencia(features, fingerprints);
            if (index < 0)
            {
                return -1;

            }
            string memberCode = memberCodes[index];
            

                string membershipType = "";
            string status = "";
            Guid customerId = Guid.Empty;


            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "SELECT membership_type, status, uuid FROM customers WHERE member_code = @member_code",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@member_code", memberCode);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            membershipType = reader.GetString(0);
                            status = reader.GetString(1);
                            customerId = reader.IsDBNull(2) ? Guid.Empty : reader.GetGuid(2);
                        }
                    }
                }
            }
            DateTime? endDate = null;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    "SELECT end_date FROM pays WHERE customer_id = @customerid ORDER BY end_date DESC LIMIT 1",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@customerid", customerId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            endDate = reader.IsDBNull(0) ? (DateTime?)null : reader.GetDateTime(0);
                        }
                    }
                }
            }


            //  Asignación FINAL
            txtNombre.Text = memberCode;
            txtTipoM.Text = membershipType;
            txtE.Text = status;
            txtFechaV.Text = endDate.HasValue
            ? endDate.Value.ToString("dd/MM/yyyy")
            : "Sin fecha";
            MessageBox.Show("Espera subiendo información a servidor ");
            RegistrarAccesoManual(
                            customerId,           // UUID del customer
                            "entry",             
                            "si"             
                            );

            return index;

           
        }
        private void RegistrarAccesoManual(
                                Guid customerId,
                                string accessType,
                                string fingerprintData
                                )
        {
            string connectionString =
            "Host=TUHOST;" +
            "Port=TUPUERTO;" +
            "Database=TUBASEDEDATOS;" +
            "Username=TUNOMBREDEUSUARIO;" +
            "Password=TUCONTRASEÑA;" +
            "SslMode=Require;" +
            "Trust Server Certificate=true;";

            int nuevoId = 1;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                //  Obtener último ID
                using (var cmd = new NpgsqlCommand(
                    "SELECT COALESCE(MAX(id), 0) FROM customer_access_logs",
                    conn))
                {
                    nuevoId = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                }

                // Insertar registro
                using (var cmd = new NpgsqlCommand(@"
            INSERT INTO customer_access_logs
            (
                id,
                customer_id,
                access_time,
                access_type,
                fingerprint_match_data,
                created_at
            )
            VALUES
            (
                @id,
                @customer_id,
                NOW(),
                @access_type,
                @fingerprint_data,
                NOW()
            )", conn))
                {
                    cmd.Parameters.AddWithValue("@id", nuevoId);
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    cmd.Parameters.AddWithValue("@access_type", accessType);
                    cmd.Parameters.AddWithValue("@fingerprint_data", fingerprintData);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Info Subida");
                }
            }
        }







        private DPFP.FeatureSet ExtraerFeaturesVerificacion(DPFP.Sample sample)
        {
            DPFP.Processing.FeatureExtraction extractor =
                new DPFP.Processing.FeatureExtraction();

            DPFP.Capture.CaptureFeedback feedback =
                DPFP.Capture.CaptureFeedback.None;

            DPFP.FeatureSet features = new DPFP.FeatureSet();

            extractor.CreateFeatureSet(
                sample,
                DPFP.Processing.DataPurpose.Verification,
                ref feedback,
                ref features
            );

            return feedback == DPFP.Capture.CaptureFeedback.Good
                ? features
                : null;
        }
        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            if (capturer != null)
            {
                try
                {
                    capturer.StopCapture();
                    capturer.EventHandler = null;
                    capturer = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al detener lector: " + ex.Message);

                }
            }
        }





        // Métodos obligatorios (aunque estén vacíos)
        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber,
            DPFP.Capture.CaptureFeedback CaptureFeedback)
        { }

        private Bitmap ConvertSampleToBitmap(DPFP.Sample sample)
        {
            DPFP.Capture.SampleConversion convertor =
                new DPFP.Capture.SampleConversion();
            Bitmap bitmap = null;
            convertor.ConvertToPicture(sample, ref bitmap);
            return bitmap;
        }

        
        private int BuscarCoincidencia(
    DPFP.FeatureSet featuresCapturados,
    List<string> fingerprints)
        {
            var verificador = new DPFP.Verification.Verification();

            for (int i = 0; i < fingerprints.Count; i++)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(fingerprints[i]);

                    using (var ms = new MemoryStream(bytes))
                    {
                        var template = new DPFP.Template();
                        template.DeSerialize(ms);

                        var result = new DPFP.Verification.Verification.Result();
                        verificador.Verify(featuresCapturados, template, ref result);

                        if (result.Verified)
                            return i; //  MATCH
                    }
                }
                catch
                {
                    // huella corrupta → la saltamos
                    continue;
                }
            }

            return -1; // ❌ No hubo coincidencia
        }

    }
}

