﻿using System;
using System.Xml.XPath;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tubumu.Modules.Framework.Swagger
{
    /// <summary>
    /// SwaggerGenOptions extensions
    /// </summary>
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
        /// </summary>
        /// <param name="swaggerGenOptions"></param>
        /// <param name="xmlDocFactory">A factory method that returns XML Comments as an XPathDocument</param>
        /// <param name="includeControllerXmlComments">
        /// Flag to indicate if controller XML comments (i.e. summary) should be used to assign Tag descriptions.
        /// Don't set this flag if you're customizing the default tag for operations via TagActionsBy.
        /// </param>
        public static void IncludeAuthorizationXmlComments(this SwaggerGenOptions swaggerGenOptions, Func<XPathDocument> xmlDocFactory, bool includeControllerXmlComments = false)
        {
            XPathDocument xpathDocument = xmlDocFactory();
            swaggerGenOptions.OperationFilter<AuthorizationXmlCommentsOperationFilter>((object)xpathDocument);
            swaggerGenOptions.SchemaFilter<XmlCommentsSchemaFilter>((object)xpathDocument);
            if (!includeControllerXmlComments)
                return;
            swaggerGenOptions.DocumentFilter<XmlCommentsDocumentFilter>((object)xpathDocument);
        }

        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
        /// </summary>
        /// <param name="swaggerGenOptions"></param>
        /// <param name="filePath">An abolsute path to the file that contains XML Comments</param>
        /// <param name="includeControllerXmlComments">
        /// Flag to indicate if controller XML comments (i.e. summary) should be used to assign Tag descriptions.
        /// Don't set this flag if you're customizing the default tag for operations via TagActionsBy.
        /// </param>
        public static void IncludeAuthorizationXmlComments(this SwaggerGenOptions swaggerGenOptions, string filePath, bool includeControllerXmlComments = false)
        {
            swaggerGenOptions.IncludeAuthorizationXmlComments((Func<XPathDocument>)(() => new XPathDocument(filePath)), includeControllerXmlComments);
        }
    }
}
