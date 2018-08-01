﻿using System;
using InvoiceCaptureLib.Connection;
using InvoiceCaptureLib.Model;
using InvoiceCaptureLib.Model.Json;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly JsonModelConverterFacade _jsonFacade;
        private readonly ApiConnectionFacade _connectionFacade;
        private readonly HttpUriBuilder _uriBuilder;


        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri) : this(new HttpUriBuilder(remoteUri), new ApiConnectionFacade(apiKey), new JsonModelConverterFacade())
        {
        }

        public InvoiceCapture(string apiKey, Uri remoteUri) : this(new HttpUriBuilder(remoteUri), new ApiConnectionFacade(apiKey), new JsonModelConverterFacade())
        {
        }

        internal InvoiceCapture(HttpUriBuilder uriBuilder,
            ApiConnectionFacade connectionFacade,
            JsonModelConverterFacade jsonFacade)
        {
            _jsonFacade = jsonFacade;
            _uriBuilder = uriBuilder;
            _connectionFacade = connectionFacade;
        }

        public Company RequestCompanyInfo()
        {
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var json = _connectionFacade.CallAPI(requestUri, "GET", "");
            return _jsonFacade.JsonToModel<Company>(json);
        }

        public Company UpdateCompanyInfo(Company company)
        {
            company.AssertHasMandatoryFields();
            var json = _jsonFacade.ModelToSendableJson(company);
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var returnedJson = _connectionFacade.CallAPI(requestUri, "PUT", json);
            return _jsonFacade.JsonToModel<Company>(returnedJson);
        }

        private Uri BuildUri(string absoluteHttpUri)
        {
            Uri uriResult;
            if (!(Uri.TryCreate(absoluteHttpUri, UriKind.Absolute, out uriResult) &&
                  (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
                throw new UriFormatException("Not a valid HTTP URI: " + absoluteHttpUri);

            return uriResult;
        }

        
    }
}