using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if(valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<List<T>>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch(Exception e)
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid value for given type");
            }
            return Task.CompletedTask;
        }
    }
}
