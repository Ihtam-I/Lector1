namespace Lector
{
    partial class Main
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonAgregarUser = new System.Windows.Forms.Button();
            this.buttonRegistrarEntrada = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAgregarUser
            // 
            this.buttonAgregarUser.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonAgregarUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonAgregarUser.Location = new System.Drawing.Point(202, 127);
            this.buttonAgregarUser.Name = "buttonAgregarUser";
            this.buttonAgregarUser.Size = new System.Drawing.Size(125, 63);
            this.buttonAgregarUser.TabIndex = 0;
            this.buttonAgregarUser.Text = "Agregar Usuario";
            this.buttonAgregarUser.UseVisualStyleBackColor = false;
            this.buttonAgregarUser.Click += new System.EventHandler(this.buttonAgregarUser_Click);
            // 
            // buttonRegistrarEntrada
            // 
            this.buttonRegistrarEntrada.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonRegistrarEntrada.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonRegistrarEntrada.Location = new System.Drawing.Point(202, 252);
            this.buttonRegistrarEntrada.Name = "buttonRegistrarEntrada";
            this.buttonRegistrarEntrada.Size = new System.Drawing.Size(125, 63);
            this.buttonRegistrarEntrada.TabIndex = 1;
            this.buttonRegistrarEntrada.Text = "Registrar Entrada";
            this.buttonRegistrarEntrada.UseVisualStyleBackColor = false;
            this.buttonRegistrarEntrada.Click += new System.EventHandler(this.buttonRegistrarEntrada_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Stencil", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(522, 38);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lector de Huellas Dactilares";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(557, 100);
            this.panel1.TabIndex = 3;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(558, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonRegistrarEntrada);
            this.Controls.Add(this.buttonAgregarUser);
            this.Name = "Main";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAgregarUser;
        private System.Windows.Forms.Button buttonRegistrarEntrada;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}

