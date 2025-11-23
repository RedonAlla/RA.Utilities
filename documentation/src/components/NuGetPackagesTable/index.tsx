import React from 'react';

type PackageListType = {
  category: string;
  packages: string[];
}

const packagesList: Array<PackageListType> = [
  {
    category: 'Core',
    packages: [
      'RA.Utilities.Core',
      'RA.Utilities.Core.Constants', 
      'RA.Utilities.Core.Exceptions',
    ]
  },
  {
    category: 'Api',
    packages: [
      'RA.Utilities.Api',
      'RA.Utilities.Api.Middlewares',
      'RA.Utilities.Api.Results',
      'RA.Utilities.Authentication.JwtBearer',
      'RA.Utilities.Authorization', 
      'RA.Utilities.OpenApi',
    ]
  },
  {
    category: 'Application',
    packages: [
      'RA.Utilities.Feature',
    ]
  },
  {
    category: 'Data',
    packages: [
      'RA.Utilities.Data.Abstractions',
      'RA.Utilities.Data.Entities',
      'RA.Utilities.Data.EntityFramework'
    ]
  },
  {
    category: 'Infrastructure',
    packages: [
      'RA.Utilities.Integrations'
    ]
  },
  {
    category: 'Logging',
    packages: [
      'RA.Utilities.Logging.Core',
      'RA.Utilities.Logging.Shared'
    ]
  },
];

const NuGetPackagesTable = () => {
  return (
    <table>
      <thead>
        <tr>
          <th>Package</th>
          <th>Version</th>
          <th>Downloads</th>
        </tr>
      </thead>
      <tbody>
        {
          packagesList.map((category, index) => (
            <React.Fragment key={index}>
              <tr>
                <th colSpan={3}>{category.category}</th>
              </tr>
              {
                category.packages.map((pkg, pckIndex) => (
                  <tr key={pckIndex}>
                    <td>
                      <a href={`https://www.nuget.org/packages/${pkg}/`}>
                        {pkg}
                      </a>
                    </td>
                    <td>
                      <img src={`https://img.shields.io/nuget/v/${pkg}.svg?logo=nuget`} alt={`${pkg} version badge`} />
                    </td>
                    <td>
                      <img src={`https://img.shields.io/nuget/dt/${pkg}.svg?logo=nuget`} alt={`${pkg} downloads badge`} />
                    </td>
                  </tr>
                ))
              }
            </React.Fragment>
          ))
        }
      </tbody>
    </table>
  );
};

export default NuGetPackagesTable;
