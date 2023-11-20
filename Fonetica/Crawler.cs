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

namespace Fonetica
{
    public class Crawler
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

        static Crawler() {
            cnn = GetOpenConnection();
        }
        public static string SQL_FCOUNTER = @"
            SELECT count(*)
		    FROM Marcas
		    WHERE LEN(Denominacion)>1 AND ExpedienteId NOT IN (SELECT ExpedienteId FROM [ip_search].[ExpTokens]);
        ";

        public static string SQL_EXPEDIENTES = @"SELECT TOP 10000 ExpedienteId, Denominacion 
                    FROM Marcas
                    WHERE LEN(Denominacion)>1 AND ExpedienteId NOT IN(SELECT ExpedienteId FROM[ip_search].[ExpTokens]);";

        // @expedienteId, @denominacion;
        public static string SQL_FONETIZER = @"
            DECLARE @wordTokenId INT;
            DECLARE @tokenId INT;

            DECLARE @expedienteId INT = CONVERT(INT, @marKId);
            DECLARE @denominacion NVARCHAR(250) = CONVERT(NVARCHAR(250), @marK);

            DECLARE @word NVARCHAR(100);

	        DECLARE words_cursor CURSOR FOR 
	        SELECT word
	        FROM SplitWords(@denominacion)

	        OPEN words_cursor
	        FETCH NEXT FROM words_cursor INTO @word;
	        WHILE @@FETCH_STATUS = 0
	        BEGIN	
		        DECLARE @lenght INT = LEN(@word);
		        DECLARE @ctrl INT=1;
		        DECLARE @position INT=1;
		        DECLARE @token nvarchar(3)='';
		        WHILE @ctrl<=@lenght
		        BEGIN
			        IF (@lenght<=3)
				        BEGIN
					        SET @token = @word;
					        SET @ctrl = @lenght;
				        END
			        ELSE
				        BEGIN
					        SET @token = SUBSTRING(@word, @ctrl, 3);
					        IF (@ctrl+2=@lenght)
						        SET @ctrl = @lenght;
				        END

			        SET @tokenId = (SELECT Id FROM [ip_search].[Tokens] WHERE Token = @token);
			        IF (@tokenId IS  NULL)
			        BEGIN
				        INSERT INTO [ip_search].[Tokens](Token) VALUES(@token);
				        SET @tokenId = (SELECT SCOPE_IDENTITY());
			        END
			        INSERT INTO [ip_search].[ExpTokens]([ExpedienteId], [TokenId], Position)
			        VALUES(@expedienteId, @tokenId, @position)
			        SET @position = @position + 1;

			        PRINT @token
			        SET @ctrl = @ctrl + 1;
		        END

		        SET @wordTokenId = (SELECT Id FROM [ip_search].[WordTokens] WHERE WordToken = @word);
		        IF (@wordTokenId IS  NULL)
		        BEGIN
			        INSERT INTO [ip_search].[WordTokens](WordToken) VALUES(@word);
			        SET @wordTokenId = (SELECT SCOPE_IDENTITY());
		        END
		        INSERT INTO [ip_search].[ExpWordTokens]([ExpedienteId], [WordTokenId])
		        VALUES(@expedienteId, @wordTokenId)

		        PRINT @word
		        FETCH NEXT FROM words_cursor INTO @word;
	        END

	        CLOSE words_cursor;
	        DEALLOCATE words_cursor;
        ";


        public static void Run()
        {

            try
            {
                bool indexing = true;
                while (indexing) {
                    var marKs = connection.Query(SQL_EXPEDIENTES);
                    if (!marKs.Any()) {
                        indexing = false;
                        continue;
                    }

                    for (int i = 0; i < marKs.Count(); i++) {
                        cnn.Execute(SQL_FONETIZER, 
                            new {
                                marKId = marKs.ElementAt(i).ExpedienteId,
                                marK = marKs.ElementAt(i).Denominacion
                            });
                        log.console(".");
                    }
                    log.console("Fonetizando marcas");
                }                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                log.Error("Error indexing marcas");
                Console.WriteLine("----");
                String x = Console.ReadLine();
            }
        }
    }
}
