﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFGetStarted.AspNetCore.ExistingDb.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace EFGetStarted.AspNetCore.ExistingDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AdventureWorks2012Context>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //  .AddIdentityServerAuthentication(options =>
            //  {
            //      options.Authority = "https://localhost:44318";
            //      options.ApiName = "AspNetCoreODataServiceApi";
            //      options.ApiSecret = "AspNetCoreODataServiceApiSecret";
            //      options.RequireHttpsMetadata = true;
            //  });

            services.AddAuthorization(options =>
                options.AddPolicy("ODataServiceApiPolicy", policy =>
                {
                    policy.RequireClaim("scope", "ScopeAspNetCoreODataServiceApi");
                })
            );

            services.AddOData();
            services.AddODataQueryFilter();

            services.AddMvc(options =>
                // https://blogs.msdn.microsoft.com/webdev/2018/08/27/asp-net-core-2-2-0-preview1-endpoint-routing/
                options.EnableEndpointRouting = false
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMvc(b =>
                   b.MapODataServiceRoute("odata", "odata", GetEdmModel(app.ApplicationServices)
            ));
        }

        private static IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(serviceProvider);

            builder.EntitySet<Person>("Person")
                .EntityType
                .Filter()
                .Count()
                .Expand()
                .OrderBy()
                .Page()
                .Select();

            EntitySetConfiguration<ContactType> contactType = builder.EntitySet<ContactType>("ContactType");
            var actionY = contactType.EntityType.Action("ChangePersonStatus");
            actionY.Parameter<string>("Level");
            actionY.Returns<bool>();

            var changePersonStatusAction = contactType.EntityType.Collection.Action("ChangePersonStatus");
            changePersonStatusAction.Parameter<string>("Level");
            changePersonStatusAction.Returns<bool>();

            EntitySetConfiguration<Person> persons = builder.EntitySet<Person>("Person");
            FunctionConfiguration myFirstFunction = persons.EntityType.Collection.Function("MyFirstFunction");
            myFirstFunction.ReturnsCollectionFromEntitySet<Person>("Person");

            //EntitySetConfiguration<EntityWithEnum> entitesWithEnum = builder.EntitySet<EntityWithEnum>("EntityWithEnum");
            //FunctionConfiguration functionEntitesWithEnum = entitesWithEnum.EntityType.Collection.Function("PersonSearchPerPhoneType");
            //functionEntitesWithEnum.Parameter<PhoneNumberTypeEnum>("PhoneNumberTypeEnum");
            //functionEntitesWithEnum.ReturnsCollectionFromEntitySet<EntityWithEnum>("EntityWithEnum");

            return builder.GetEdmModel();
        }
    }
}
