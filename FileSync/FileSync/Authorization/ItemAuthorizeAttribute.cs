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
        private bool _allowEmpty;

        public ItemAuthorizeAttribute(string itemType, bool allowEmpty = false) : base()
        {
            _itemType = itemType;
            _allowEmpty = allowEmpty;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var baseResult = base.AuthorizeCore(httpContext);
            if (!baseResult) 
                return baseResult;

            if (!httpContext.Request.RequestContext.RouteData.Values.ContainsKey("id"))
                return _allowEmpty;

            var itemId = httpContext.Request.RequestContext.RouteData.Values["id"].ToString();

            var identity = httpContext.User.Identity;
            IAuthorizableItem item = null;
            if(_itemType == "file")
            {
                item = FileSyncDal.GetFile(identity, itemId);
            }
            else if(_itemType == "folder")
            {
                item = FileSyncDal.GetFolder(identity, itemId);
            }
            
            if(item == null) 
                return false;

            return ItemAuthorizer.IsAuthorized(identity, item);
        }
    }
}