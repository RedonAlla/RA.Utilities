import type {ReactNode} from 'react';
import Link from '@docusaurus/Link';
import './styles.scss';

type Package = {
  name: string;
  shortName: string;
  description: string;
  link: string;
  layer: 'core' | 'api' | 'data' | 'application' | 'infrastructure' | 'logging';
};

const packages: Package[] = [
  {
    name: 'RA.Utilities.Core',
    shortName: 'Core',
    description: 'Result monad, implicit conversions, and functional patterns for predictable error handling without exceptions.',
    link: '/nuget-packages/core/RA.Utilities.Core/',
    layer: 'core',
  },
  {
    name: 'RA.Utilities.Core.Constants',
    shortName: 'Constants',
    description: 'Centralized HTTP status codes, response messages, header names, and the extensible ResponseType record.',
    link: '/nuget-packages/core/RA.Utilities.Core.Constants/',
    layer: 'core',
  },
  {
    name: 'RA.Utilities.Core.Exceptions',
    shortName: 'Exceptions',
    description: 'Semantic exception hierarchy (NotFound, Conflict, BadRequest...) with built-in HTTP status mapping.',
    link: '/nuget-packages/core/RA.Utilities.Core.Exceptions/',
    layer: 'core',
  },
  {
    name: 'RA.Utilities.Api',
    shortName: 'Api',
    description: 'Endpoint auto-discovery, global exception handling, and typed HTTP response infrastructure.',
    link: '/nuget-packages/api/RA.Utilities.Api/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.Api.Results',
    shortName: 'Api.Results ⚠️',
    description: 'Deprecated — merged into RA.Utilities.Api. All response types now available under the RA.Utilities.Api.Results namespace.',
    link: '/nuget-packages/api/RA.Utilities.Api/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.Api.Middlewares',
    shortName: 'Middlewares',
    description: 'Request ID propagation, default headers, and cross-cutting HTTP middleware components.',
    link: '/nuget-packages/api/RA.Utilities.Api.Middlewares/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.OpenApi',
    shortName: 'OpenApi',
    description: 'Swagger/OpenAPI operation transformers for automatic error response documentation.',
    link: '/nuget-packages/api/RA.Utilities.OpenApi/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.Authentication.JwtBearer',
    shortName: 'JwtBearer',
    description: 'Pre-configured JWT Bearer authentication with sensible defaults and easy customization.',
    link: '/nuget-packages/auth/AuthenticationJwtBearer/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.Authorization',
    shortName: 'Authorization',
    description: 'Policy-based authorization utilities and claim-based access control helpers.',
    link: '/nuget-packages/auth/Authorization/',
    layer: 'api',
  },
  {
    name: 'RA.Utilities.Feature',
    shortName: 'Feature',
    description: 'Custom mediator with CQRS pipeline behaviors — lighter than MediatR, purpose-built for vertical slices.',
    link: '/nuget-packages/Application/Feature/',
    layer: 'application',
  },
  {
    name: 'RA.Utilities.Application.Validation',
    shortName: 'Validation',
    description: 'FluentValidation integration with pipeline behaviors for automatic request validation.',
    link: '/nuget-packages/Application/FeatureValidation/',
    layer: 'application',
  },
  {
    name: 'RA.Utilities.Data.Entities',
    shortName: 'Data.Entities',
    description: 'Base entity classes with common properties (Id, CreatedAt, LastModifiedAt) and audit support.',
    link: '/nuget-packages/Data/Entities/',
    layer: 'data',
  },
  {
    name: 'RA.Utilities.Data.Abstractions',
    shortName: 'Data.Abstractions',
    description: 'Repository interfaces with read/write separation — compose only what your aggregates need.',
    link: '/nuget-packages/Data/Abstractions/',
    layer: 'data',
  },
  {
    name: 'RA.Utilities.Data.EntityFramework',
    shortName: 'Data.EF',
    description: 'EF Core repository implementations, save-change interceptors, and registration extensions.',
    link: '/nuget-packages/Data/EntityFramework/',
    layer: 'data',
  },
  {
    name: 'RA.Utilities.Integrations',
    shortName: 'Integrations',
    description: 'Typed HTTP client factory with delegating-handler pipeline: logging, auth, proxy, header forwarding.',
    link: '/nuget-packages/Integrations/',
    layer: 'infrastructure',
  },
  {
    name: 'RA.Utilities.Logging.Core',
    shortName: 'Logging.Core',
    description: 'Serilog enrichment and configuration utilities for structured logging consistency.',
    link: '/nuget-packages/Logging/RA.Utilities.Logging.Core/',
    layer: 'logging',
  },
  {
    name: 'RA.Utilities.Logging.Shared',
    shortName: 'Logging.Shared',
    description: 'Shared log templates, request-id enrichment, and HTTP logging message formats.',
    link: '/nuget-packages/Logging/RA.Utilities.Logging.Shared/',
    layer: 'logging',
  },
];

const layerConfig: Record<string, {label: string; color: string; icon: string}> = {
  core: {
    label: 'Core',
    color: '#512bd4',
    icon: '◆',
  },
  api: {
    label: 'Api',
    color: '#3e80ed',
    icon: '⬡',
  },
  application: {
    label: 'Application',
    color: '#10b981',
    icon: '◇',
  },
  data: {
    label: 'Data',
    color: '#f59e0b',
    icon: '◈',
  },
  infrastructure: {
    label: 'Infrastructure',
    color: '#ef4444',
    icon: '▣',
  },
  logging: {
    label: 'Logging',
    color: '#8b5cf6',
    icon: '◉',
  },
};

function PackageCard({pkg}: {pkg: Package}) {
  const config = layerConfig[pkg.layer];
  return (
    <Link to={pkg.link} className="package-card-link">
      <article className={`package-card package-card--${pkg.layer}`}>
        <div className="package-card__header">
          <span className="package-card__icon" style={{color: config.color}}>
            {config.icon}
          </span>
          <span className="package-card__layer" style={{color: config.color, borderColor: `${config.color}33`}}>
            {config.label}
          </span>
        </div>
        <h3 className="package-card__name">{pkg.shortName}</h3>
        <p className="package-card__desc">{pkg.description}</p>
        <span className="package-card__arrow" style={{color: config.color}}>→</span>
      </article>
    </Link>
  );
}

export default function PackageShowcase(): ReactNode {
  return (
    <section className="package-showcase">
      <div className="container">
        <header className="package-showcase__heading">
          <p className="package-showcase__eyebrow">17 NuGet Packages</p>
          <h2 className="package-showcase__title">
            Everything you need to build <span>clean .NET APIs</span>
          </h2>
          <p className="package-showcase__subtitle">
            A curated suite of packages organized by architectural layer — pick what you need,
            compose them together, and focus on business logic instead of infrastructure.
          </p>
        </header>
        <div className="package-showcase__grid">
          {packages.map((pkg) => (
            <PackageCard key={pkg.name} pkg={pkg} />
          ))}
        </div>
      </div>
    </section>
  );
}
