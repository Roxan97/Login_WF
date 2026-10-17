using RepoDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    public partial class FrmUsuario : Form
    {
       
        private readonly string _connectionString  = ConfigurationManager.ConnectionStrings["conexionBD"].ConnectionString;

        public FrmUsuario()
        {
            InitializeComponent();
            
        }

        //Variables Globales
        private int usuarioSeleccionadoId = 0;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CargarUsuarios()
        {
            using (var uow = new UnitOfWork(_connectionString))
            {
                var lista = uow.Usuarios.GetByEstado(true).ToList();
                dataUsuarios.DataSource = null;
                dataUsuarios.DataSource = lista;
                dataUsuarios.Columns["Id"].Visible = false;
                dataUsuarios.Refresh();
            }
        }

        private void LimpiarControles ()
        {
            txtNombre.Clear();
            txtEmail.Clear();
            txtCodigo.Clear();
            txtClave.Clear();

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId > 0)
            {
                MessageBox.Show("Este usuario solo se puede Actualizar.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtClave.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }
            var clave = txtClave.Text.Trim();

            //  Encriptamos la clave antes de guardar
            var claveEncriptada = Encriptador.EncriptarClave(clave);

            var usuario = new Usuarios
            {
                Nombre = txtNombre.Text,
                Email = txtEmail.Text,
                Clave = claveEncriptada,
                Codigo = txtCodigo.Text,
                FechaCreacion = DateTime.Now,
                Activo=true,
                FechaModificacion = DateTime.Now
            };

            using (var uow = new UnitOfWork(_connectionString))
            {
                uow.Usuarios.Insert(usuario);
                uow.Commit();
            }

            MessageBox.Show("Usuario guardado correctamente.");

            LimpiarControles();
            CargarUsuarios();
           
        }

        private void FrmUsuario_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
            txtClave.UseSystemPasswordChar = true; // activa la máscara
            txtClave.CharacterCasing = CharacterCasing.Lower; // convierte a minuscula
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
        }

        private void dataUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var usuario = (Usuarios)dataUsuarios.Rows[e.RowIndex].DataBoundItem;

                using (var formEditar = new FrmUsuarioCmd(usuario.Id, _connectionString))
                {
                    var result = formEditar.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        CargarUsuarios(); // recargar el grid si hubo cambios
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un usuario para actualizar.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var usuario = new Usuarios
            {
                Id = usuarioSeleccionadoId,
                Nombre = txtNombre.Text,
                Email = txtEmail.Text,
                FechaModificacion=DateTime.Now
            };

            var camposActualizar = new List<Field>();

            if (!string.IsNullOrWhiteSpace(txtNombre.Text))
                camposActualizar.Add(new Field("Nombre"));

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                camposActualizar.Add(new Field("Email"));

            camposActualizar.Add(new Field("FechaModificacion"));

            using (var uow = new UnitOfWork(_connectionString))
            {
                if (camposActualizar.Any())
                {
                    uow.Usuarios.Update(usuario, camposActualizar);
                    uow.Commit();
                    MessageBox.Show("Usuario actualizado correctamente.");
                    LimpiarControles();
                    usuarioSeleccionadoId = 0;
                    CargarUsuarios(); // recargar la grilla
                }
                else
                {
                    MessageBox.Show("No se proporcionaron cambios para actualizar.");
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.");
                return;
            }
         var confirmar = MessageBox.Show("¿Está seguro que desea eliminar este usuario?","Confirmar eliminación",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );

            if (confirmar != DialogResult.OK)
            {
                return; // El usuario canceló
            }


            var usuario = new Usuarios
            {
                Id = usuarioSeleccionadoId,
                Activo = false,
                FechaModificacion = DateTime.Now
            };

            var camposActualizar = new List<Field>();

        

            camposActualizar.Add(new Field("FechaModificacion"));
            camposActualizar.Add(new Field("Activo"));

            using (var uow = new UnitOfWork(_connectionString))
            {
                if (camposActualizar.Any())
                {
                    uow.Usuarios.Update(usuario, camposActualizar);
                    uow.Commit();
                    MessageBox.Show("Usuario eliminado correctamente.");
                    LimpiarControles();
                    usuarioSeleccionadoId = 0;
                    CargarUsuarios(); // recargar la grid
                }
                else
                {
                    MessageBox.Show("No se completo la operacion");
                }
            }

        }

        private void dataUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var usuario = (Usuarios)dataUsuarios.Rows[e.RowIndex].DataBoundItem;

                usuarioSeleccionadoId = usuario.Id;
                txtNombre.Text= usuario.Nombre;
                txtEmail.Text= usuario.Email;
            }


        }
    }
}
