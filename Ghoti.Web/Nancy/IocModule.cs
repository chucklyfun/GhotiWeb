using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Game;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Ninject.Extensions.Factory;
using ghoti.web.Controllers;
using Nancy;
using Nancy.Serialization.JsonNet;
using System.Reflection;
using Utilities;
using GameLogic.Domain;

namespace Ghoti.Web
{
    public class IocModule : IIocModule
    {
        public bool Bind(IKernel kernel)
        {
            kernel.Bind<ICrudController<User>>().To<CrudController<User>>();
            kernel.Bind<ICrudController<Game>>().To<CrudController<Game>>();

            return true;
        }
    }
}