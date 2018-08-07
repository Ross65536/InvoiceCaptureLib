﻿using System;
using System.Collections.Generic;
using System.Text;
using InvoiceCaptureLib.Model;
using Newtonsoft.Json;

namespace test.Model
{
    class ModelBuilder
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatString = "yyyy'-'MM'-'dd"
        };

        private const string VatNumber = "510205933"; // is actually valid in pt
        private const string Id = "4567";

        private IDictionary<string, object> _fields = new Dictionary<string, object>();

        public ModelBuilder()
        {
        }

        public ModelBuilder(IDictionary<string, object> fields)
        {
            _fields = new Dictionary<string, object>(fields);
        }

        public object this[string key]
        {
            set => _fields[key] = value;

            get
            {
                _fields.TryGetValue(key, out var value);
                return value;
            }
        }

        public T BuildModel<T>()
            where T : InvoiceCaptureLib.Model.Model, new()
        {
            return new T() { Fields = new Dictionary<string, object>(_fields)};
        }

        public string BuildJson()
        {
            return JsonConvert.SerializeObject(_fields, SerializerSettings);
        }

        // should only add the id
        public static ModelBuilder BuildReplyCompanyBuilder()
        {
            var builder = BuildRequestCompanyBuilder();
            builder[Company.IdName] = Id;

            return builder;
        }

        public static ModelBuilder BuildRequestCompanyBuilder()
        {
            var fields = new Dictionary<string, object>()
            {
                { Company.VatNumberName, VatNumber },
                { Company.NameName, "a name" },
                { Company.ZipCodeName, null },
            };

            return new ModelBuilder(fields);
        }

        // should only add the id
        public static ModelBuilder BuildReplyCustomerBuilder()
        {
            var builder = BuildRequestCustomerBuilder();
            builder[Customer.IdName] = Id;

            return builder;
        }

        public static ModelBuilder BuildRequestCustomerBuilder()
        {
            var fields = new Dictionary<string, object>()
            {
                { Customer.VatNumberName, VatNumber },
                { Customer.NameName, "a name" },
                { Customer.ZipCodeName, null },
                { Customer.CountryName, "PT" },
            };

            return new ModelBuilder(fields);
        }

        public static string DictToJson(IDictionary<string, string> dict)
        {
            return JsonConvert.SerializeObject(dict, SerializerSettings);
        }

        public ModelBuilder Clone()
        {
            return new ModelBuilder(_fields);
        }
    }
}
