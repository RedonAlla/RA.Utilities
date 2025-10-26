import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const defaultLocale = 'en';

const config: Config = {
  title: 'RA.Utilities',
  tagline: 'RA.Utilities is a "batteries-included" framework for building modern .NET APIs. It provides the foundation for a clean architecture so you can focus more on writing business logic and less on setting up infrastructure.',
  favicon: 'img/favicon.ico',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  // Set the production url of your site here
  url: 'https://redonalla.github.io',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/RA.Utilities/',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'RedonAlla', // Usually your GitHub org/user name.
  projectName: 'RA.Utilities', // Usually your repo name.
  deploymentBranch: "gh-pages",

  onBrokenLinks: 'throw',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        debug: process.env.NODE_ENV === 'development',
        docs: {
          path: 'docs',
          routeBasePath: '/nuget-packages',
          sidebarPath: './sidebars.ts',
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl: ({locale, versionDocsDirPath, docPath}) => {
            return `https://github.com/RedonAlla/RA.Utilities/tree/main/documentation/${docPath}`;
          },

        },
        blog: {
          showReadingTime: true,
          feedOptions: {
            type: ['rss', 'atom'],
            xslt: true,
          },
          // Useful options to enforce blogging best practices
          onInlineTags: 'warn',
          onInlineAuthors: 'warn',
          onUntruncatedBlogPosts: 'warn',
        },
        theme: {
          customCss: './src/css/custom.scss',
        },
      } satisfies Preset.Options,
    ],
  ],

  themes: [
    [
      // @ts-ignore
      require.resolve("@easyops-cn/docusaurus-search-local"),
      /** @type {import("@easyops-cn/docusaurus-search-local").PluginOptions} */
      // @ts-ignore
      ({
        hashed: true,
        indexDocs: true,
        indexBlog: true,
        indexPages: false,
        docsDir: [ "docs" ],
        docsRouteBasePath: [
          "/nuget-packages",
          "/vs-template"
        ],
        blogDir: [ "changeLogs" ],
        blogRouteBasePath: [
          // "/blog", 
          "/changelogs"
        ],
        language: ["en"],
        searchBarShortcut: true,
        searchBarShortcutHint: true,
        explicitSearchResultPath: true,
        searchResultLimits: 10,
        highlightSearchTermsOnTargetPage: true,
      }),
    ],
  ],

  plugins: [
    'docusaurus-plugin-sass',
    [
      '@docusaurus/plugin-content-docs',
      {
        id: 'vsTemplate',
        path: 'vsTemplate',
        routeBasePath: 'vs-template',
        sidebarPath: './sidebars.ts',
      },
    ],
    [
      '@docusaurus/plugin-content-blog',
      {
          /**
           * Required for any multi-instance plugin
           */
          id: 'changeLogs',
          /**
           * URL route for the blog section of your site.
           */
          routeBasePath: 'changelogs',
          /**
           * Path to data on filesystem relative to site dir.
           */
          path: 'changeLogs',
          blogTitle: 'Change Logs',
          blogSidebarTitle: 'Change Logs',
          blogSidebarCount: 'ALL',
          blogDescription: 'Lis of Change Logs for RA.Utilities packages',
          feedOptions: {
            type: 'all',
            copyright: `Copyright © ${new Date().getFullYear()} Redon Alla.`,
          },
        },
    ]
  ],

  themeConfig: {
    // Replace with your project's social card
    image: 'img/docusaurus-social-card.jpg',
    metadata: [
      {name: 'theme-color', content: '#e50d00'},
      {name: 'keywords', content: '.NET, API, framework, clean architecture, utilities, error handling, Result type'},
    ],
    announcementBar: {
      id: 'work_in_progress',
      content: '⭐️ If you like RA.Utilities, give it a star on <a target="_blank" rel="noopener noreferrer" href="https://github.com/RedonAlla/RA.Utilities">GitHub</a>! ⭐️',
    },
    colorMode: {
      respectPrefersColorScheme: true,
    },
    docs: {
      sidebar: {
        hideable: true,
        autoCollapseCategories: false,
      },
    },
    navbar: {
      title: 'RA.Utilities',
      logo: {
        alt: 'RA.Utilities Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          to: '/nuget-packages/intro',
          sidebarId: 'nugetPackagesSidebar',
          position: 'left',
          label: 'Nuget Packages',
          activeBaseRegex: `/nuget-packages/`,
          className: 'menu_item_icon vs_template_menu_item',
        },
        {
          to: '/vs-template/intro',
          position: 'left',
          label: 'VS Template',
          activeBaseRegex: `/vs-template/`,
          className: 'menu_item_icon nuget_menu_item',
        },
        // {to: '/blog', label: 'Blog', position: 'left'},
        {to: '/changelogs', label: 'Change Logs', position: 'left'},
        {
          href: 'https://github.com/RedonAlla/RA.Utilities',
          position: 'right',
          className: 'header-github-link',
          'aria-label': 'GitHub repository',
        },
      ],
    },
    footer: {
      style: 'light',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Nuget packages',
              to: '/nuget-packages/intro',
            },
            {
              label: 'VS Templates',
              to: 'vs-template/intro'
            }
          ],
        },
        {
          title: 'Community',
          items: [
            {
              label: 'Stack Overflow',
              href: 'https://stackoverflow.com/questions/tagged/docusaurus',
            },
            {
              label: 'Discord',
              href: 'https://discordapp.com/invite/docusaurus',
            },
            {
              label: 'X',
              href: 'https://x.com/docusaurus',
            },
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'Blog',
              to: '/blog',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/facebook/docusaurus',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Redon Alla. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.vsLight,
      darkTheme: prismThemes.vsDark,
      additionalLanguages: [
        'powershell',
        'bash',
        'diff',
        'json',
        'csharp'
      ],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
