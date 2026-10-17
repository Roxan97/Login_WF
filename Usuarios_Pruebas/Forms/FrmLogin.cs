using RepoDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuarios_Pruebas.Clases;
using Usuarios_Pruebas.Repositorios;

namespace Usuarios_Pruebas.Forms
{
    public partial class FrmLogin : Form
    {
      
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(txtClave.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Debe ingresar usuario y contraseña");
                return;
            }

           
       
            // Buscar en la base
            using (var uow = new UnitOfWork(_connectionString))
            {
                string codigo = txtCodigo.Text.Trim();

                    var clave = txtClave.Text.Trim();
                    var claveEncriptada = Encriptador.EncriptarClave(clave);

                    var usuario = uow.Usuarios.ObtenerPorCredenciales(codigo, claveEncriptada);

                if (usuario != null)
                {
                    MessageBox.Show($"¡Bienvenido {usuario.Nombre}!", "Acceso permitido",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de acceso",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = true; // activa la máscara
            txtClave.CharacterCasing = CharacterCasing.Lower; // convierte a minuscula
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
        }
    }
}
