import type {ReactNode} from 'react';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import PackageShowcase from '@site/src/components/PackageShowcase';
import HomepageFeatures from '@site/src/components/HomepageFeatures';

import styles from './index.module.css';

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={siteConfig.title}
      description='RA.Utilities is a "batteries-included" framework for building modern .NET APIs. It provides the foundation for a clean architecture so you can focus more on writing business logic and less on setting up infrastructure.'>
      <PackageShowcase />
      <main>
        <HomepageFeatures />
      </main>
    </Layout>
  );
}
