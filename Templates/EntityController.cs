﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DatabaseScriptsGenerator.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using DatabaseScriptsGenerator;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class EntityController : EntityControllerBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using AddAppAPI.Helpers;\r\nusing AddAppAPI.Models;\r\nusing System;\r\nusing System.Co" +
                    "llections.Generic;\r\nusing System.Net;\r\nusing System.Threading.Tasks;\r\nusing Syst" +
                    "em.Web.Http;\r\n\r\nnamespace AddAppAPI.Controllers\r\n{\r\n    [RoutePrefix(\"api/");
            
            #line 11 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("\")]\r\n    public class ");
            
            #line 12 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("Controller : ApiController\r\n    {\r\n        private ISqlHelper sqlHelper;\r\n\r\n     " +
                    "   public ");
            
            #line 16 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("Controller()\r\n        {\r\n            this.sqlHelper = new SqlHelper();\r\n        }" +
                    "\r\n\r\n        public ");
            
            #line 21 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("Controller(ISqlHelper sqlHelper)\r\n        {\r\n            this.sqlHelper = sqlHelp" +
                    "er;\r\n        }\r\n\r\n");
            
            #line 26 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
if(!string.IsNullOrEmpty(this.KeyColumnCategory) && this.KeyColumnCategory.Equals("primary")){
            
            #line default
            #line hidden
            this.Write("        // GET: api/");
            
            #line 27 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("\r\n        public async Task<IHttpActionResult> Get()\r\n        {\r\n            var " +
                    "");
            
            #line 30 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.PluralCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" = await this.sqlHelper.SelectAllAsync<");
            
            #line 30 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(">();\r\n            return Content(HttpStatusCode.OK, ");
            
            #line 31 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.PluralCaseEntityName));
            
            #line default
            #line hidden
            this.Write(");\r\n        }\r\n\r\n        // GET: api/");
            
            #line 34 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("/5\r\n        public async Task<IHttpActionResult> Get(");
            
            #line 35 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(" id)\r\n        {\r\n\t\t\tvar parameters = new List<Parameter>();\r\n\t\t\tparameters.Add(ne" +
                    "w Parameter { Name = \"id\", Value = id });\r\n            var ");
            
            #line 39 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" = await this.sqlHelper.SelectOneAsync<");
            
            #line 39 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(">(parameters);\r\n            if (");
            
            #line 40 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" != null)\r\n            {\r\n                return Content(HttpStatusCode.OK, ");
            
            #line 42 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(");\r\n            }\r\n            else\r\n            {\r\n                var message =" +
                    " string.Format(\"No ");
            
            #line 46 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(" found for ");
            
            #line 46 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" id:{0}\", id);\r\n                return Content(HttpStatusCode.NotFound, message);" +
                    "\r\n            }\r\n        }\r\n\r\n");
            
            #line 51 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
if(this.FkColumns.Count > 0 && this.HasIdColumn){
            
            #line default
            #line hidden
            
            #line 52 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
foreach (var c in this.FkColumns){
	var methodName = string.Format("Get{0}IdsBy{1}", this.EntityName, c.ReferencingColumnName);

            
            #line default
            #line hidden
            this.Write("\t\t[HttpGet]\r\n\t\t[Route(\"");
            
            #line 56 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(methodName));
            
            #line default
            #line hidden
            this.Write("\")]\r\n        public async Task<IHttpActionResult> ");
            
            #line 57 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(methodName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 57 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture($"{c.DotNetType} id"));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n\t\t\tvar parameters = new List<Parameter>();\r\n\t\t\tparameters.Add(new P" +
                    "arameter { Name = \"");
            
            #line 60 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(c.ReferencingColumnName));
            
            #line default
            #line hidden
            this.Write("\", Value = id });\r\n\t\t\tList<");
            
            #line 61 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write("> ids = await this.sqlHelper.SelectIdsAsync<");
            
            #line 61 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 61 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(">(parameters);\r\n\t\t\treturn Content(HttpStatusCode.OK, ids);\r\n        }\r\n");
            
            #line 64 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}}
            
            #line default
            #line hidden
            
            #line 65 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
if(HasNameColumn){
            
            #line default
            #line hidden
            this.Write("\r\n        // GET: api/");
            
            #line 67 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("/GetIdNamepairs\r\n        [HttpGet]\r\n        [Route(\"GetIdNamepairs\")]\r\n        pu" +
                    "blic async Task<IHttpActionResult> GetIdNamepairs()\r\n        {\r\n            var " +
                    "idNamePairs = await this.sqlHelper.SelectIdNamePairsAsync<");
            
            #line 72 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 72 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(", string>();\r\n            return Content(HttpStatusCode.OK, idNamePairs);\r\n      " +
                    "  }\r\n\r\n");
            
            #line 76 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}else{
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 78 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}
            
            #line default
            #line hidden
            this.Write("\t\t// PUT: api/");
            
            #line 79 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("/5\r\n        public async Task<IHttpActionResult> Put(");
            
            #line 80 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(" id, ");
            
            #line 80 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 80 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            if (id != ");
            
            #line 82 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(".Id)\r\n            {\r\n                var message = string.Format(\"id:\'{0}\' not ma" +
                    "tching with ");
            
            #line 84 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" id:\'{1}\'\", id, ");
            
            #line 84 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(".Id);\r\n                return Content(HttpStatusCode.BadRequest, message);\r\n     " +
                    "       }\r\n\r\n            var success = await this.sqlHelper.UpdateAsync<");
            
            #line 88 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(">(");
            
            #line 88 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(");\r\n            if (success)\r\n            {\r\n                return Content(HttpS" +
                    "tatusCode.OK, id);\r\n            }\r\n            else\r\n            {\r\n            " +
                    "    var message = string.Format(\"No ");
            
            #line 95 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(" found for ");
            
            #line 95 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(" id:\'{0}\'\", id);\r\n                return Content(HttpStatusCode.NotFound, message" +
                    ");\r\n            }\r\n        }\r\n");
            
            #line 99 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}
            
            #line default
            #line hidden
            this.Write("\r\n        // POST: api/");
            
            #line 101 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("\r\n        public async Task<IHttpActionResult> Post(");
            
            #line 102 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 102 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n");
            
            #line 104 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
if(!string.IsNullOrEmpty(this.KeyColumnCategory) && this.KeyColumnCategory.Equals("primary")){
            
            #line default
            #line hidden
            this.Write("            ");
            
            #line 105 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(" id = await this.sqlHelper.InsertOneAsync<");
            
            #line 105 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 105 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnType));
            
            #line default
            #line hidden
            this.Write(">(");
            
            #line 105 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(");\r\n            return Content(HttpStatusCode.OK, id);\r\n");
            
            #line 107 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}else{
            
            #line default
            #line hidden
            this.Write("\t\t\tawait this.sqlHelper.InsertOneAsync<");
            
            #line 108 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(", object>(");
            
            #line 108 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.LowerCaseEntityName));
            
            #line default
            #line hidden
            this.Write(");\r\n            return Ok();\r\n");
            
            #line 110 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n        // DELETE: api/");
            
            #line 113 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write("/5\r\n        public async Task<IHttpActionResult> Delete(");
            
            #line 114 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnAndDotNetTypeList));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n\t\t\tvar parameters = new List<Parameter>();\r\n");
            
            #line 117 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
foreach(var c in KeyColumns){
            
            #line default
            #line hidden
            this.Write("\t\t\tparameters.Add(new Parameter { Name = \"");
            
            #line 118 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(c.Name));
            
            #line default
            #line hidden
            this.Write("\", Value = ");
            
            #line 118 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(c.DotNetVariableName));
            
            #line default
            #line hidden
            this.Write(" });\r\n\r\n");
            
            #line 120 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
}
            
            #line default
            #line hidden
            this.Write("            var success = await this.sqlHelper.DeleteAsync<");
            
            #line 121 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(">(parameters);\r\n            if (success)\r\n            {\r\n                return O" +
                    "k();\r\n            }\r\n            else\r\n            {\r\n                var messag" +
                    "e = string.Format(\"No ");
            
            #line 128 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.EntityName));
            
            #line default
            #line hidden
            this.Write(" found for given ");
            
            #line 128 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.KeyColumnDotNetVariableNameList));
            
            #line default
            #line hidden
            this.Write("\");\r\n                return Content(HttpStatusCode.NotFound, message);\r\n         " +
                    "   }\r\n        }\r\n    }\r\n}\r\n\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 142 "C:\Users\raambat\Documents\Visual Studio 2017\Projects\AddWinFormsApp\DatabaseScriptsGenerator\Templates\EntityController.tt"

public string EntityName { get; set; }
public string LowerCaseEntityName { get; set; }
public string PluralCaseEntityName { get; set; }
public string KeyColumnType { get; set; }
public string KeyColumnCategory { get; set; }
public bool HasNameColumn { get; set; }
public List<ColumnInfo> KeyColumns { get; set; }
public string KeyColumnAndDotNetTypeList { get; set; }
public string KeyColumnDotNetVariableNameList { get; set; }
public bool HasIdColumn { get; set; }
public List<ColumnInfo> FkColumns { get; set; }

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class EntityControllerBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
