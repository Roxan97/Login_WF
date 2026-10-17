using RepoDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuarios_Pruebas.Clases;
using Usuarios_Pruebas.Repositorios;

namespace Usuarios_Pruebas.Forms
{
    public partial class FrmUsuarioCmd : Form
    {
        private readonly string _connectionString;
        private readonly int _usuarioId;
        public FrmUsuarioCmd(int usuarioId, string connectionString)
        {
            InitializeComponent();
            _usuarioId = usuarioId;
            _connectionString = connectionString;
        }

        private void FrmUsuarioCmd_Load(object sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = true; // activa la máscara
            txtClave.CharacterCasing = CharacterCasing.Lower; // convierte a minuscula
            txtCodigo.CharacterCasing = CharacterCasing.Upper;

            chbActivo.Enabled = false;
            using (var uow = new UnitOfWork(_connectionString))
            {
                var usuario = uow.Usuarios.GetById(_usuarioId);

                if (usuario == null)
                {
                    MessageBox.Show("Usuario no encontrado.");
                    this.Close();
                    return;
                }

                txtNombre.Text = usuario.Nombre;
                txtEmail.Text = usuario.Email;
                chbActivo.Checked = usuario.Activo;
                txtClave.Text = usuario.Clave;
                txtCodigo.Text = usuario.Codigo;
            }

        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtClave.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var clave = txtClave.Text.Trim();
            var claveEncriptada = Encriptador.EncriptarClave(clave);

            var usuario = new Usuarios
            {
                Id = _usuarioId,
                Nombre = txtNombre.Text,
                Email = txtEmail.Text,
                Clave = claveEncriptada,
                Codigo = txtCodigo.Text,
                FechaModificacion = DateTime.Now
            };

            var camposActualizar = new List<Field>
            {
                new Field("Nombre"),
                new Field("Email"),
                new Field("Clave"),
                new Field("Codigo"),
                new Field("FechaModificacion")
            };

            using (var uow = new UnitOfWork(_connectionString))
            {
                uow.Usuarios.Update(usuario, camposActualizar);
                uow.Commit();
                MessageBox.Show("Usuario actualizado correctamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    
}
