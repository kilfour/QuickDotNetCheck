using System;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using QuickDotNetCheck.ElaborateExample.Domain;

namespace QuickDotNetCheck.ElaborateExample.Tests.DataAccess
{
    public abstract class DatabaseTest : IDisposable
    {
        private static Configuration configuration;
        private static ISessionFactory sessionFactory;

        public ISession NHibernateSession { get; set; }
        private readonly ITransaction transaction;

        protected DatabaseTest()
        {
            if (configuration == null)
            {
                FileHelper.DeletePreviousDbFiles();
                var dbFile = FileHelper.GetDbFileName();
                configuration = new Configuration();
                configuration.Configure();
                configuration.DataBaseIntegration(
                    db =>
                        {
                            db.ConnectionString = string.Format("Data Source={0};Version=3;New=True;", dbFile);
                        });
                var definition = new DomainDefinition();
                configuration.AddDeserializedMapping(definition.Mapping(), definition.GetType().Name);
                var schemaExport = new SchemaExport(configuration);
                schemaExport.Create(true, true);
                sessionFactory = configuration.BuildSessionFactory();
            }
            NHibernateSession = sessionFactory.OpenSession();
            transaction = NHibernateSession.BeginTransaction();
        }

        public void Dispose()
        {
            transaction.Rollback();
            transaction.Dispose();
            NHibernateSession.Dispose();
        }

        protected void SaveToSession<T>(T entity)
        {
            NHibernateSession.Save(entity);
        }

        protected void FlushAndClear()
        {
            NHibernateSession.Flush();
            NHibernateSession.Clear();
        }
    }

    public class FileHelper
    {
        public static string GetDbFileName()
        {
            var path = Path.GetFullPath(Path.GetRandomFileName() + ".Test.db");
            return !File.Exists(path) ? path : GetDbFileName();
        }

        public static void DeletePreviousDbFiles()
        {
            var files = Directory.GetFiles(".", "*.Test.db*");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
