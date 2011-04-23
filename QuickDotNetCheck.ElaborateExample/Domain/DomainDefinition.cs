using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace QuickDotNetCheck.ElaborateExample.Domain
{
    public class DomainDefinition
    {
        public HbmMapping Mapping()
        {
            var mapper = new ConventionModelMapper();
            mapper.Class<Person>(map => map.Id(person => person.Id, id => id.Generator(Generators.Identity)));
            return mapper.CompileMappingForAllExplicitAddedEntities();
        }
    }
}