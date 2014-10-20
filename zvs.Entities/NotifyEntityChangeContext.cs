using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    public abstract class NotifyEntityChangeContext : DbContext
    {
        protected NotifyEntityChangeContext()
        {
        }

        protected NotifyEntityChangeContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public static class ChangeNotifications<T>
        {
            public static event OnEntityAddedEventHandler OnEntityAdded = delegate { };
            public delegate void OnEntityAddedEventHandler(object sender, EntityAddedArgs e);

            public static event OnEntityUpdatedEventHandler OnEntityUpdated = delegate { };
            public delegate void OnEntityUpdatedEventHandler(object sender, EntityUpdatedArgs e);

            public static event OnEntityDeletedEventHandler OnEntityDeleted = delegate { };
            public delegate void OnEntityDeletedEventHandler(object sender, EntityDeletedArgs e);

            public static void RaiseEntityAdded(object sender, object addedEntity)
            {
                OnEntityAdded(sender, new EntityAddedArgs(addedEntity));
            }

            public static void RaiseEntityUpdated(object sender, object newEntity, object oldEntity)
            {
                OnEntityUpdated(sender, new EntityUpdatedArgs(newEntity, oldEntity));
            }

            public static void RaiseEntityDeleted(object sender, object deletedEntity)
            {
                OnEntityDeleted(sender, new EntityDeletedArgs(deletedEntity));
            }

            #region Event Args
            public class EntityAddedArgs : EventArgs
            {
                public T AddedEntity { get; private set; }

                public EntityAddedArgs(object addedEntity)
                {
                    AddedEntity = (T)addedEntity;
                }
            }

            public class EntityUpdatedArgs : EventArgs
            {
                public T NewEntity { get; private set; }
                public T OldEntity { get; private set; }

                public EntityUpdatedArgs(object newEntity, object oldEntity)
                {
                    NewEntity = (T)newEntity;
                    OldEntity = (T)oldEntity;
                }
            }

            public class EntityDeletedArgs : EventArgs
            {
                public T DeletedEntity { get; private set; }

                public EntityDeletedArgs(object deletedEntity)
                {
                    DeletedEntity = (T)deletedEntity;
                }
            }
            #endregion

        }

        public static void RaiseEntityAdded(object sender, object addedEntity)
        {
            var raiseEntityAdded = typeof(ChangeNotifications<>).MakeGenericType(addedEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityAdded");
            raiseEntityAdded.Invoke(null, new[] { sender, addedEntity });
        }

        public static void RaiseEntityUpdated(object sender, object newEntity, object oldEntity)
        {
            var raiseEntityUpdated = typeof(ChangeNotifications<>).MakeGenericType(newEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityUpdated");
            raiseEntityUpdated.Invoke(null, new[] { sender, newEntity, oldEntity });
        }

        public static void RaiseEntityDeleted(object sender, object deletedEntity)
        {
            var raiseEntityDeleted = typeof(ChangeNotifications<>).MakeGenericType(deletedEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityDeleted");
            raiseEntityDeleted.Invoke(null, new[] { sender, deletedEntity });
        }

        public async override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            //Record Changes
            var addedEntities = new List<object>();
            var modifedEntities = new List<dynamic>();
            var deletedEntities = new List<object>();

            ChangeTracker.Entries().ToList().ForEach(o =>
            {
                switch (o.State)
                {
                    case EntityState.Added:
                        addedEntities.Add(o.Entity);
                        break;
                    case EntityState.Modified:
                        modifedEntities.Add(new
                        {
                            NewEntity = o.Entity,
                            OldEntity = o.OriginalValues.ToObject()
                        });
                        break;
                    case EntityState.Deleted:
                        deletedEntities.Add(o.Entity);
                        break;
                }
            });

            //Try Save Changes
            var saveChangesResult = await base.SaveChangesAsync(cancellationToken);

            //Report changes if SaveChanges did not throw an error.
            addedEntities.ForEach(o => RaiseEntityAdded(this, o));

            modifedEntities.ForEach(o => RaiseEntityUpdated(this, o.NewEntity, o.OldEntity));

            deletedEntities.ForEach(o => RaiseEntityDeleted(this, o));

            return saveChangesResult;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }

        
    }

    public static class EntityFrameworkExtentions
    {
        public static Type GetTypeEntityWrapperDetection(this Object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            return obj.HasEntityWrapper() ? obj.GetType().BaseType : obj.GetType();
        }

        public static bool HasEntityWrapper(this Object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            var ns = obj.GetType().Namespace;
            return ns != null && (ns.StartsWith("System.Data.Entity.Dynamic"));
        }
    }
}
