using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    public abstract class NotifyEntityChangeContext : DbContext
    {
        //Constructors
        public NotifyEntityChangeContext() : base() { }
        public NotifyEntityChangeContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        /// <summary>
        /// Use these events for cross-context notifications
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class ChangeNotifications<T>
        {
            public static event onEntityAddedEventHandler onEntityAdded = delegate { };
            public delegate void onEntityAddedEventHandler(object sender, EntityAddedArgs e);

            public static event onEntityUpdatedEventHandler onEntityUpdated = delegate { };
            public delegate void onEntityUpdatedEventHandler(object sender, EntityUpdatedArgs e);

            public static event onEntityDeletedEventHandler onEntityDeleted = delegate { };
            public delegate void onEntityDeletedEventHandler(object sender, EntityDeletedArgs e);

            public static void RaiseEntityAdded(object sender, object addedEntity)
            {
                onEntityAdded(sender, new EntityAddedArgs(addedEntity));
            }

            public static void RaiseEntityUpdated(object sender, object newEntity, object oldEntity)
            {
                onEntityUpdated(sender, new EntityUpdatedArgs(newEntity, oldEntity));
            }

            public static void RaiseEntityDeleted(object sender, object deletedEntity)
            {
                onEntityDeleted(sender, new EntityDeletedArgs(deletedEntity));
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
            MethodInfo RaiseEntityAdded = typeof(ChangeNotifications<>).MakeGenericType(addedEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityAdded");
            RaiseEntityAdded.Invoke(null, new object[] { sender, addedEntity });
        }

        public static void RaiseEntityUpdated(object sender, object newEntity, object oldEntity)
        {
            MethodInfo RaiseEntityUpdated = typeof(ChangeNotifications<>).MakeGenericType(newEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityUpdated");
            RaiseEntityUpdated.Invoke(null, new object[] { sender, newEntity, oldEntity });
        }

        public static void RaiseEntityDeleted(object sender, object deletedEntity)
        {
            MethodInfo RaiseEntityDeleted = typeof(ChangeNotifications<>).MakeGenericType(deletedEntity.GetTypeEntityWrapperDetection()).GetMethod("RaiseEntityDeleted");
            RaiseEntityDeleted.Invoke(null, new object[] { sender, deletedEntity });
        }

        public async override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            //Record Changes
            List<object> AddedEntities = new List<object>();
            List<dynamic> ModifedEntities = new List<dynamic>();
            List<object> DeletedEntities = new List<object>();

            this.ChangeTracker.Entries().ToList().ForEach(o =>
            {
                if (o.State == EntityState.Added)
                    AddedEntities.Add(o.Entity);
                else if (o.State == EntityState.Modified)
                    ModifedEntities.Add(new
                    {
                        NewEntity = o.Entity,
                        OldEntity = o.OriginalValues.ToObject()
                    });
                else if (o.State == EntityState.Deleted)
                    DeletedEntities.Add(o.Entity);
            });

            //Try Save Changes
            var saveChangesResult = await base.SaveChangesAsync(cancellationToken);

            //Report changes if SaveChanges did not throw an error.
            AddedEntities.ForEach(o =>
            {
                RaiseEntityAdded(this, o);
            });

            ModifedEntities.ForEach(o =>
            {
                RaiseEntityUpdated(this, o.NewEntity, o.OldEntity);
            });

            DeletedEntities.ForEach(o =>
            {
                RaiseEntityDeleted(this, o);
            });

            return saveChangesResult;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }
    }

    public static class EFExtensionHelpers
    {
        public static Type GetTypeEntityWrapperDetection(this Object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            if (obj.hasEntityWrapper())
                return obj.GetType().BaseType;
            else
                return obj.GetType();
        }

        public static bool hasEntityWrapper(this Object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            if (obj.GetType().Namespace != null && obj.GetType().Namespace.StartsWith("System.Data.Entity.Dynamic"))
                return true;

            return false;
        }
    }
}
