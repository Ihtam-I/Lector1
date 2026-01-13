using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DPFP;


namespace Lector
{

    public partial class Main : Form, DPFP.Capture.EventHandler
    {
        

        private DPFP.Capture.Capture capturer;
        private bool formularioAbierto = false;
        

        public Main()
        {
            InitializeComponent();
            this.Load += Main_Load;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            IniciarCaptura();
        }

        private void buttonAgregarUser_Click(object sender, EventArgs e)
        {
            if (formularioAbierto) return;

            formularioAbierto = true;
            DetenerCaptura();

            frmAgregarUsuario frm = new frmAgregarUsuario();

            frm.FormClosed += (s, ev) =>
            {
                formularioAbierto = false;
                IniciarCaptura(); //  vuelve a escuchsr
            };

            frm.Show();
        }



        private void buttonRegistrarEntrada_Click(object sender, EventArgs e)
        {
            if (formularioAbierto) return;

            formularioAbierto = true;

            //  Apagamos lector antes de abrir
            DetenerCaptura();

            FrmRegEntrada frm = new FrmRegEntrada();

            frm.FormClosed += (s, ev) =>
            {
                formularioAbierto = false;
                IniciarCaptura();   // vuelve a escuchar
            };

            frm.Show();
        }
        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            if (formularioAbierto) return;

            formularioAbierto = true;

            BeginInvoke(new Action(() =>
            {
                DetenerCaptura();

                FrmRegEntrada frm = new FrmRegEntrada();

                frm.FormClosed += (s, e) =>
                {
                    formularioAbierto = false;
                    IniciarCaptura(); // vuelvea escuchar
                };

                frm.Show();
            }));
        }


        private void ResetLector()
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

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }

        public void OnFingerGone(object Capture, string ReaderSerialNumber) { }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber) { }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) { }

        public void OnSampleQuality(
            object Capture,
            string ReaderSerialNumber,
            DPFP.Capture.CaptureFeedback CaptureFeedback
        )
        { }

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
                MessageBox.Show("Error lector: " + ex.Message);
            }
        }

        private void DetenerCaptura()
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
        }





    }
}