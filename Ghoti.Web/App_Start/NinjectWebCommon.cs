namespace Ghoti.Web.App_Start
{
    using System;
    using System.Web;


    using Ninject;
    using Ghoti.Web.Nancy;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        public static StandardKernel Kernel { get; set; }
        

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
        }        
    }
}
