﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using InvoiceCaptureLib.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string InvoicesEndPoint = "invoices";
        private const string CreditNoteEndPoint = "credit_notes";
        private const string PaymentsEndPoint = "payments";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";

        private string _remoteUri;

        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri)
        {
            ApiKey = apiKey;
            RemoteUri = remoteUri;
        }

        public string ApiKey { get; set; }

        public string RemoteUri
        {
            get => _remoteUri;

            set
            {
                if (checkURI(value))
                    _remoteUri = value;
                else
                    throw new UriFormatException("Not a valid URI");
            }
        }

        public Company RequestCompanyInfo()
        {
            var json = callAPI(CompanyEndPoint, "", "GET");
            return JsonConvert.DeserializeObject<Company>(json);
        }

        public Company UpdateCompanyInfo(Company company)
        {
            var jsonString = JsonConvert.SerializeObject(company,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(CompanyEndPoint, jsonString, "PUT");
            return JsonConvert.DeserializeObject<Company>(jsonString);
        }

        private string callAPI(string endpoint, string jsonString, string method)
        {
            var response = "";
            var client = getWebClient();
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    response = client.UploadString(RemoteUri + endpoint, method, jsonString);
                else if (method == "GET")
                    response = client.DownloadString(RemoteUri + endpoint);
            }
            catch (WebException e)
            {
                var responseStream = e.Response?.GetResponseStream();
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        response = reader.ReadToEnd();
                        var error = JsonConvert.DeserializeObject<InvoiceCaptureError>(response);
                        throw new InvoiceCaptureException(error.Code + " " + error.Message);
                    }
            }

            return response;
        }

        private bool checkURI(string value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private WebClient getWebClient()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new WebClient();
            client.Headers.Set("Content-Type", "application/json");
            client.Headers.Set("X-Api-Token", ApiKey);
            client.Encoding = Encoding.UTF8;
            return client;
        }


        // unrefactored, unprocessed methods
        public Customer createCustomer(Customer c)
        {
            var jsonString = JsonConvert.SerializeObject(c,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(CustomerEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public Debt createInvoice(Debt i)
        {
            var client = getWebClient();
            var s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonString = JsonConvert.SerializeObject(i, s);
            jsonString = callAPI(InvoicesEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<Debt>(jsonString);
        }

        public Customer getCustomer(string id)
        {
            var jsonString = callAPI(CustomerEndPoint + "/" + id, "", "GET");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public List<Debt> getCustomerDebts(string id)
        {
            var jsonString = callAPI(CustomerEndPoint + "/" + id + "/debts", "", "GET");
            return JsonConvert.DeserializeObject<List<Debt>>(jsonString);
        }

        public Debt getInvoice(string id)
        {
            var jsonString = callAPI(InvoicesEndPoint + "/" + id, "", "GET");
            return JsonConvert.DeserializeObject<Debt>(jsonString);
        }

        public Customer updateCustomer(Customer c, string id)
        {
            var jsonString = JsonConvert.SerializeObject(c,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(CustomerEndPoint + "/" + id, jsonString, "PUT");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }
    }
}