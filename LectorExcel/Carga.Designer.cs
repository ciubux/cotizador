namespace LectorExcel
{
    partial class Carga
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cargarclientes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cargarclientes
            // 
            this.cargarclientes.Location = new System.Drawing.Point(29, 36);
            this.cargarclientes.Name = "cargarclientes";
            this.cargarclientes.Size = new System.Drawing.Size(155, 23);
            this.cargarclientes.TabIndex = 0;
            this.cargarclientes.Text = "Cargar Clientes";
            this.cargarclientes.UseVisualStyleBackColor = true;
            this.cargarclientes.Click += new System.EventHandler(this.button1_Click);
            // 
            // Carga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.cargarclientes);
            this.Name = "Carga";
            this.Text = "Carga";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cargarclientes;
    }
}