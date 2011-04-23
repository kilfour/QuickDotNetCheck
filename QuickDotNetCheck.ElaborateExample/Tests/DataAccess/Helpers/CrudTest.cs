using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickGenerate;
using QuickGenerate.Reflect;
using Xunit;

namespace QuickDotNetCheck.ElaborateExample.Tests.DataAccess
{
    public abstract class CrudTest<TEntity, TId> : DatabaseTest
        where TEntity : IHaveAnId<TId>
        where TId : IComparable<TId>
    {
        protected abstract DomainGenerator GenerateAndSaveGenerator();

        protected CrudTest()
        {
            properties = typeof(TEntity).GetProperties().ToList();
        }

        protected void Has_A<T, TChildId>(Expression<Func<TEntity, T>> expression)
            where T : IHaveAnId<TChildId>
        {
            var entity = BuildEntity();

            NHibernateSession.Flush();

            var entityId = entity.Id;
            var childId = expression.Compile().Invoke(entity).Id;

            NHibernateSession.Clear();

            entity = NHibernateSession.Get<TEntity>(entityId);

            Assert.Equal(childId, expression.Compile().Invoke(entity).Id);
        }

        protected void Has_A<T, TChildId>(Expression<Func<TEntity, T>> expression,
            Action<TEntity, T> func)
            where T : IHaveAnId<TChildId>
        {
            var entity =
                GenerateAndSaveGenerator()
                .OneToOne(func).One<TEntity>();

            NHibernateSession.Flush();

            var entityId = entity.Id;
            var childId = expression.Compile().Invoke(entity).Id;

            NHibernateSession.Clear();

            entity = NHibernateSession.Get<TEntity>(entityId);

            Assert.Equal(childId, expression.Compile().Invoke(entity).Id);
        }

        protected void Has_Many<TMany, TChildId>(Expression<Func<TEntity, IEnumerable<TMany>>> expression)
            where TMany : IHaveAnId<TChildId>
        {
            var entity = BuildEntity();
            NHibernateSession.Flush();
            var manies = expression.Compile().Invoke(entity).ToList();
            Assert.False(manies.Count() == 0, "no entities in many relation");
            var entityId = entity.Id;
            var ids = manies.Select(many => many.Id).ToList();
            NHibernateSession.Clear();
            entity = NHibernateSession.Get<TEntity>(entityId);
            manies = expression.Compile().Invoke(entity).ToList();
            foreach (var many in manies)
            {
                Assert.True(ids.Contains(many.Id));
            }
        }

        protected void Has_Many<TMany, TChildId>(
            Expression<Func<TEntity, IEnumerable<TMany>>> expression,
            Action<TEntity, TMany> func)
            where TMany : IHaveAnId<TChildId>
        {
            var entity = 
                GenerateAndSaveGenerator()
                .OneToMany(3, func).One<TEntity>();

            NHibernateSession.Flush();
            var manies = expression.Compile().Invoke(entity).ToList();
            Assert.False(manies.Count() == 0, "no entities in many relation");
            var entityId = entity.Id;
            var ids = manies.Select(many => many.Id).ToList();
            NHibernateSession.Clear();
            entity = NHibernateSession.Get<TEntity>(entityId);
            manies = expression.Compile().Invoke(entity).ToList();
            foreach (var many in manies)
            {
                Assert.True(ids.Contains(many.Id));
            }
        }

        [Fact]
        public virtual void SelectQueryWorks()
        {
            NHibernateSession.CreateCriteria(typeof(TEntity)).SetMaxResults(5).List();
        }

        [Fact]
        public virtual void AddEntity_EntityWasAdded()
        {
            var entity = BuildEntity();
            NHibernateSession.Flush();
            NHibernateSession.Evict(entity);
            var reloadedEntity = NHibernateSession.Get<TEntity>(entity.Id);
            Assert.NotNull(reloadedEntity);
            AssertEqual(entity, reloadedEntity);
            Assert.True(entity.Id.CompareTo(default(TId)) != 0);
        }

        [Fact]
        public virtual void UpdateEntity_EntityWasUpdated()
        {
            var entity = BuildEntity();
            NHibernateSession.Flush();
            var id = entity.Id;
            ModifyEntity(entity);
            entity.Id = id;
            UpdateEntity(entity);
            NHibernateSession.Evict(entity);
            var reloadedEntity = NHibernateSession.Get<TEntity>(entity.Id);
            Assert.NotNull(reloadedEntity);
            AssertEqual(entity, reloadedEntity);
        }

        [Fact]
        public virtual void DeleteEntity_EntityWasDeleted()
        {
            var entity = BuildEntity();
            NHibernateSession.Flush();
            DeleteEntity(entity);
            Assert.Null(NHibernateSession.Get<TEntity>(entity.Id));
        }

        protected virtual TEntity BuildEntity()
        {
            var generator = GenerateAndSaveGenerator();
            var entity = generator.One<TEntity>();
            return entity;
        }

        protected virtual TEntity ModifyEntity(TEntity entity)
        {
            GenerateAndSaveGenerator().ModifyThis(entity).ChangeAll();
            return entity;
        }

        protected virtual void UpdateEntity(TEntity entity)
        {
            NHibernateSession.Update(entity);
            NHibernateSession.Flush();
        }

        protected virtual void DeleteEntity(TEntity entity)
        {
            NHibernateSession.Delete(entity);
            NHibernateSession.Flush();
        }

        private readonly List<PropertyInfo> properties;

        protected void DontAssert<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyName = propertyExpression.AsMemberExpression().Member.Name;
            properties.RemoveAll(src => src.Name == propertyName);
        }

        protected virtual void AssertEqual<T>(Type type, T expectedEntity, T actualEntity)
        {
            type.GetProperties()
                .ToList()
                .ForEach(src => VerifyEqualityOf(src, expectedEntity, actualEntity));
        }

        protected virtual void AssertEqual<T>(T expectedEntity, T actualEntity)
        {
            properties.ForEach(src => VerifyEqualityOf(src, expectedEntity, actualEntity));
        }

        private static void VerifyEqualityOf<T>(PropertyInfo src, T expectedEntity, T actualEntity)
        {
            var errorMessage = string.Format(
                "{2}{0}.{1}{2}  Expected : {3}.{2}  Actual : {4}.{2}",
                typeof(TEntity).Name,
                src.Name,
                Environment.NewLine,
                src.GetValue(expectedEntity, null),
                src.GetValue(actualEntity, null));

            Assert.True(
                Equals(src.GetValue(expectedEntity, null), src.GetValue(actualEntity, null)), errorMessage);
        }
    }
}