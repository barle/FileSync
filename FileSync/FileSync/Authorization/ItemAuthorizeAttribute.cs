using FileSync.DAL;
using FileSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileSync.Authorization
{
    public class ItemAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var baseResult = base.AuthorizeCore(httpContext);
            if (!baseResult) 
                return baseResult;

            var identity = httpContext.User.Identity;
            IAuthorizableItem item = null;
            if(httpContext.Request.Params.Keys.OfType<string>().Contains("fileId"))
            {
                var fileId = int.Parse(httpContext.Request.Params.Get("fileId"));
                item = FileSyncDal.GetFile(identity, fileId);
            }
            else if(httpContext.Request.Params.Keys.OfType<string>().Contains("folderId"))
            {
                var folderId = int.Parse(httpContext.Request.Params.Get("folderId"));
                item = FileSyncDal.GetFolder(identity, folderId);
            }
            
            if(item == null) 
                return false;

            return ItemAuthorizer.IsAuthorized(identity, item);
        }
    }
}