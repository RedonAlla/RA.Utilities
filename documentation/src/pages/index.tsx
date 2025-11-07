import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import HomepageFeatures from '@site/src/components/HomepageFeatures';
import Heading from '@theme/Heading';

import styles from './index.module.css';
import Illustration from '../../static/img/BannerIllustration.svg';

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={styles.heroBanner}>
      <div className="container">
        <div className="row">
          <div className="col col--8">
          <Heading as="h3" className="hero__title">
            {siteConfig.title}
          </Heading>
            <p>{siteConfig.tagline}</p>
          </div>
          <div className="col col--4">
            <Illustration width={'25rem'} height={'25rem'} />
          </div>
        </div>
      </div>

      {/* <div className="container">
        <Heading as="h1" className="hero__title">
          {siteConfig.title}
        </Heading>
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link
            className="button button--secondary button--lg"
            to="/nuget-packages/intro">
            Docusaurus Tutorial - 5min ⏱️
          </Link>
        </div>
      </div> */}
    </header>
  );
}

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={siteConfig.title}
      description='RA.Utilities is a "batteries-included" framework for building modern .NET APIs. It provides the foundation for a clean architecture so you can focus more on writing business logic and less on setting up infrastructure.'>
      <HomepageHeader />
      <main>
        <HomepageFeatures />
      </main>
    </Layout>
  );
}
