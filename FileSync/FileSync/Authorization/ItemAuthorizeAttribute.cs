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
        private string _itemType;

        public ItemAuthorizeAttribute(string itemType) : base()
        {
            _itemType = itemType;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var baseResult = base.AuthorizeCore(httpContext);
            if (!baseResult) 
                return baseResult;

            var identity = httpContext.User.Identity;
            IAuthorizableItem item = null;
            if(_itemType == "file")
            {
                var fileId = httpContext.Request.RequestContext.RouteData.Values["id"].ToString();
                item = FileSyncDal.GetFile(identity, fileId);
            }
            else if(_itemType == "folder")
            {
                var folderId = httpContext.Request.RequestContext.RouteData.Values["id"].ToString();
                item = FileSyncDal.GetFolder(identity, folderId);
            }
            
            if(item == null) 
                return false;

            return ItemAuthorizer.IsAuthorized(identity, item);
        }
    }
}