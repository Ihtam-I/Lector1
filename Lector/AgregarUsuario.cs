using DPFP;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lector
{
    public partial class frmAgregarUsuario : Form
    {
        private void frmAgregarUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            Template = null;
        }

        private DPFP.Template Template;
        public frmAgregarUsuario()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            frmAgregarhuella mostrarHuella = new frmAgregarhuella();
            mostrarHuella.OnTemplate += this.OnTemplate;
            mostrarHuella.ShowDialog(); 
        }
        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate () {
                Template = template;
                btnAgregar.Enabled = (Template != null);
                if (Template != null)
                {
                    MessageBox.Show("Huella Capturada con exito");

                }
                else
                {
                    MessageBox.Show("Huella No capturada");
                }


            }));

            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Template == null)
            {
                MessageBox.Show("No hay huella capturada");
                return;
            }

            string huellaBase64 = TemplateToBase64(Template);

            string connectionString =
            "Host=TUHOST;" +
            "Port=TUPUERTO;" +
            "Database=TUBASEDEDATOS;" +
            "Username=TUNOMBREDEUSUARIO;" +
            "Password=TUCONTRASEÑA;" +
            "SslMode=Require;" +
            "Trust Server Certificate=true;";

            string query = @"
                UPDATE customers
                SET fingerprint_data = @huella
                WHERE member_code = @id;
                ";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        //int id = int.Parse(textBox2.Text);

                        cmd.Parameters.AddWithValue("@huella", huellaBase64);
                        cmd.Parameters.AddWithValue("@id", textBox2.Text); // ← cambia por tu ID real

                        int filasAfectadas = cmd.ExecuteNonQuery(); 

                        if (filasAfectadas == 0)                     
                        {
                            MessageBox.Show(
                                "Member Code no encontrado.\nVerifica el código.",
                                "Atención",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;                                  
                        }
                    }
                }

                MessageBox.Show("Huella guardada correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string TemplateToBase64(DPFP.Template template)
        {
            using (var ms = new MemoryStream())
            {
                template.Serialize(ms);
                byte[] bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }


        private void frmAgregarUsuario_Load(object sender, EventArgs e)
        {

        }
    }
}
