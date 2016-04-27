﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gistlyn.Common.Interfaces;
using Gistlyn.Common.Objects;
using Gistlyn.ServiceModel;
using NuGet;
using ServiceStack;

namespace Gistlyn.ServiceInterface
{
    public class NugetService : Service
    {
        public WebHostConfig Config { get; set; }

        public IDataContext DataContext { get; set; }

        public object Any(SearchNugetPackages request)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            var packages = repo.Search(request.Search, request.AllowPrereleaseVersion)
                               .Take(50)
                               .ToList();

            List<NugetPackageInfo> packageInfos = packages
                                                  .Select(p => new NugetPackageInfo() { Id = p.Id, Version = p.Version.Version, Ver = p.Version.Version.ToString()})
                                                  .ToList();

            return new SearchNugetPackagesResponse()
            {
                Packages = packageInfos
            };
        }

        public object Any(InstallNugetPackage request)
        {
            NugetHelper.InstallPackage(DataContext, Config.NugetPackagesDirectory, request.PackageId, request.Ver);

            return new InstallNugetPackageResponse();
        }

        public object Any(AddPackageAsReference request)
        {
            return new AddPackageAsReferenceResponse()
            {
                Assemblies = NugetHelper.RestorePackage(DataContext, Config.NugetPackagesDirectory, request.PackageId, request.Version)
            };
        }

        public object Any(SearchInstalledPackages request)
        {
            List<NugetPackageInfo> packages = DataContext.SearchPackages(request.Search, null);

            return new SearchInstalledPackagesResponse()
            {
                Packages = packages
            };
        }
    }
}

