using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios_Pruebas.Repositorios
{
    public interface IUnitOfWork : IDisposable
    {
        UsuarioRepository Usuarios { get; }
        void Commit();
    }
}
