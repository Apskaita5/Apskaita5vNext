using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.MicroOrm;

namespace DeveloperUtils.TestClasses
{
    public abstract class ChildBaseClass<T> where T : ChildBaseClass<T>
    {

        private bool _isNew = true;


        public bool IsNew => _isNew;


        protected ChildBaseClass() { }


        public async Task Save(IOrmService service, object parentId)
        {
            BeforeSave();
            if (_isNew)
            {
                await service.ExecuteInsertAsync<T>((T)this, GetParamsForParentId(parentId));
                SaveChildren();
                _isNew = false;
            }
            else
            {
                await service.ExecuteUpdateAsync<T>((T)this);
                SaveChildren();
            }
        }

        public async Task Delete(IOrmService service)
        {
            if (_isNew)
            {
                return;
            }
            else
            {
                await service.ExecuteDeleteAsync((T)this);
                _isNew = true;
            }
        }

        public static async Task<T> Fetch(IOrmService service, object id)
        {
            var result = (T)Activator.CreateInstance(typeof(T));
            await service.LoadObjectFieldsAsync<T>(result, id, CancellationToken.None);
            result._isNew = false;
            return result;
        }

        public static async Task<List<T>> FetchChildList(IOrmService service, object parentId)
        {
            var data = await service.FetchTableAsync<T>(parentId, true, CancellationToken.None);

            var result = new List<T>();

            foreach (var dr in data.Rows)
            {
                var newItem = (T)Activator.CreateInstance(typeof(T));
                service.LoadObjectFields(newItem, dr);
                newItem._isNew = false;
                result.Add(newItem);
            }

            return result;

        }
        

        protected virtual void BeforeSave() { }

        protected virtual void SaveChildren() { }

        protected abstract SqlParam[] GetParamsForParentId(object parentId);

    }
}
