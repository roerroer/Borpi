using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Security.Cryptography;

namespace DataImport
{
    public class Usuarios
    {
        public static string ConnectionString;

        public static SqlConnection connection;

        public static SqlConnection GetOpenConnection()
        {
            ConnectionString = ConfigurationManager.AppSettings["rpiDb"];
            if (connection == null)
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
            }
            return connection;
        }

        private static SqlConnection cnn = GetOpenConnection();

        static Usuarios() {
            cnn = GetOpenConnection();
        }

        public class legacyUsuario
        {
            public int USUARIO_ID { get; set; }
            public string EMAIL { get; set; }
            public string ABC_PASS { get; set; }
            public string NOMBRE { get; set; }
            public string EMPRESA { get; set; }
            public string DIR_EMPRESA { get; set; }
            public string TELEFONOS { get; set; }
            public string PAIS_ID { get; set; }
            public bool SUSCRIPCION { get; set; }
            public DateTime? fecha_suscripcion { get; set; }
            public DateTime? fecha_caducidad { get; set; }
        }

        public class CryptoManager 
        {
            private byte[] _salt = System.Text.Encoding.Default.GetBytes("Gestion.p.i.");

            public string Encrypt(string text)
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(text + _salt);

                SHA1 sha = new SHA1CryptoServiceProvider();
                var result = sha.ComputeHash(data);
                return System.Text.Encoding.Default.GetString(result);
            }
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static void ImportUsuarios()
        {
            CryptoManager _cryptoManager = new CryptoManager();

            var data = connection.Query<legacyUsuario>("SELECT * FROM externo.usuarios");

            foreach (legacyUsuario u in data) {
                try
                {
                    string pw = _cryptoManager.Encrypt(u.ABC_PASS.Trim());

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = u.PAIS_ID.Trim() }).FirstOrDefault();
                    string key1 = RandomString(4);
                    string key2 = RandomString(4);
                    string fullkey = key1 + "-" + key2;
                    
                    DateTime spkExpiration = DateTime.Now.AddHours(72);

                    cnn.Execute(@"INSERT INTO [dbo].[UsuariosPublicos]
                           ([Cuenta]
                           ,[Pwd]
                           ,[Nombre]
                           ,[Empresa]
                           ,[Direccion]
                           ,[Ciudad]
                           ,[EstadoProvincia]
                           ,[Telefonos]
                           ,[PaisId]
                           ,[Suscripcion]
                           ,[Spk]
                           ,[SpkExpiration]
                           ,[FechaSuscripcion]
                           ,[FechaCaducidad])
                           VALUES(
                            @Cuenta
                           ,@Pwd
                           ,@Nombre
                           ,@Empresa
                           ,@Direccion
                           ,@Ciudad
                           ,@EstadoProvincia
                           ,@Telefonos
                           ,@PaisId
                           ,@Suscripcion
                           ,@Spk
                           ,@SpkExpiration
                           ,@FechaSuscripcion
                           ,@FechaCaducidad
                            )",
                            new
                            {
                                Cuenta = u.EMAIL.Trim(),
                                Pwd = pw,
                                Nombre = u.NOMBRE.Trim(),
                                Empresa = u.EMPRESA.Trim(),
                                Direccion = u.DIR_EMPRESA.Trim(),
                                Ciudad = string.Empty,
                                EstadoProvincia = string.Empty,
                                Telefonos = u.TELEFONOS.Trim(),
                                PaisId = paisId,
                                Suscripcion = false,
                                Spk = fullkey,
                                SpkExpiration = spkExpiration,
                                FechaSuscripcion = u.fecha_suscripcion,
                                FechaCaducidad = u.fecha_caducidad,
                            }
                    );

                    log.Error("Cuenta:" + u.EMAIL);
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Cuenta {0} tiene errores", u.EMAIL));
                    log.Error(e.Message);
                }
            }
        }
    }
}
