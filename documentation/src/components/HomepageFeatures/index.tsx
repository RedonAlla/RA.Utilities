import type {ReactNode} from 'react';
import Link from '@docusaurus/Link';
import clsx from 'clsx';
import './styles.scss';

type FeatureItem = {
  title: string;
  icon: string;
  description: string;
  link: string;
};

const FeatureList: FeatureItem[] = [
  {
    title: 'NuGet',
    icon: 'nuget',
    link: '/nuget-packages/intro',
    description: 'NuGet packages designed to accelerate modern .NET API development. These packages provide reusable, production-ready building blocks to solve common problems, allowing you to focus on business logic instead of boilerplate code. Easily integrate into your projects via NuGet for fast setup and consistent behavior.',
  },
  {
    title: 'Project Template',
    icon: 'visual_studio',
    link: 'vs-template/intro',
    description: "A powerful foundation for building applications using ASP.NET Core and Clean Architecture principles. It promotes a structured, maintainable, and scalable development workflow by separating concerns and enforcing clear boundaries. Designed to reduce complexity and improve long-term performance, it empowers teams to deliver robust solutions with confidence and efficiency. Whether you're launching a new product or modernizing legacy systems, this template gives you the confidence and clarity to build software that lasts."
  },
  {
    title: 'Item Templates',
    icon: 'template',
    link: '/nuget-packages/intro',
    description: "RA.Utilities includes a curated set of item templates that supercharge your development workflow. These templates provide ready-to-use scaffolding for common components—like Endpoints, Features, and DTOs—so you can focus on writing meaningful code, not boilerplate.  RA.Utilities item templates help you move faster with confidence."
  },
];

function Feature({title, icon, link, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className={clsx('single-services', icon, 'text--center')} >
          <div className="services-icon align-items-center justify-content-center">
            <i />
          </div>
          <div className="services-content">
            <h4 className="services-title">
              <Link to={link}>{title}</Link>
            </h4>
            <p className="text"> { description }</p>
          </div>
        </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className='services-area'>
        <div className="container">
          <div className="row">
            {FeatureList.map((props, idx) => (
              <Feature key={idx} {...props} />
            ))}
          </div>
        </div>
    </section>
  );
}
