using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text.Json;

namespace HandMadeStore.UI
{
    public class JsonStringLocalizer : IStringLocalizer

    {
         private readonly IDistributedCache _cash;
        private readonly Newtonsoft.Json.JsonSerializer _serializer = new ();

      

        public JsonStringLocalizer(IDistributedCache Cash)
        {
            _cash = Cash;
        }

        public LocalizedString this[string name]

        {
            get
            {

                var value = GetString(name);

                return new LocalizedString(name, value);
            }
        }

            

        public LocalizedString this[string name, params object[] arguments]

        {
            get
            {

                var actualvalue = this[name];

                return !actualvalue.ResourceNotFound
                    ? new LocalizedString(name,  String.Format(actualvalue.Value,arguments))
                    
                    : actualvalue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            // قراءة البيانات في فايل الجيسون 
            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);


            using StreamReader stremmReader = new(stream);
            // قراءة فايل الجيسون
            using JsonTextReader reader = new(stremmReader);



            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName )
                {
                    continue;
                      var key=reader.Value as string;
                    reader.Read();
                                  var value = _serializer.Deserialize<string>(reader);

                                   yield return new LocalizedString(key,value);
                }
            }
                    }

        //  filepath لمعرفة مكان فايل الجيسون الي راح نقرى منه البيانات
        // propertyname  الكي الي راح تقراه
        private string GetValueFromJson(string propertyname, string filepath)
        {
            if(string.IsNullOrEmpty(propertyname) || string.IsNullOrEmpty(filepath)) { 
            

                return string.Empty;
            }
            
            // قراءة البيانات في فايل الجيسون 
            using FileStream stream=new(filepath, FileMode.Open,FileAccess.Read,FileShare.Read); 


            using StreamReader stremmReader = new (stream);
            // قراءة فايل الجيسون
            using JsonTextReader reader = new (stremmReader);

          //  قراء فايل الجيسون

            while(reader.Read())
            {
                if(reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyname)
                {

                    reader.Read();
                    // تحويل من جيسون الى سترنج
                    return _serializer.Deserialize<string>(reader);
                }


            }

            return string.Empty;
        }


        private string GetString(string key)
        {
            // Resources/ar-EG.json
            // Resources/en-US.json
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            // اتاكد من ان المسار موجود فعلا
            var fullFilePath = Path.GetFullPath(filePath);

            if (File.Exists(fullFilePath))
            {

                // get cash Key
                // $ String Interpolation   
                // locale_en_US_welcome
                // locale_ar_EG_welcome
                var cashkey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cashvalue = _cash.GetString(cashkey);
                // في حالة انه تكون مخزونة في الكاش
                if (!string.IsNullOrEmpty(cashvalue))
                {
                        // رجعلي القيمة المخزنة عندك في الكاش
                    return cashvalue;

                }
                   // في حال لم تكن مخزنة في الكاش
                        // يرجع القراءة من فايل الجيسون
                    var result= GetValueFromJson(key, fullFilePath);
                // والان اضيفها في الكاش
                if (!string.IsNullOrEmpty(result) )
                {
                    _cash.SetString(cashkey, result);   

                }

             
                return result;
            }
            return string.Empty;
        }

    }
}
