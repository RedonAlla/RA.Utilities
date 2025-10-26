import React, { useEffect } from 'react';
import { useColorMode } from '@docusaurus/theme-common';

const LIGHT_THEME_COLOR = 'rgb(37, 194, 160)';
const DARK_THEME_COLOR = 'rgb(27, 27, 29)'; // A common dark background color

function updateThemeColor(color: string) {
  let meta = document.querySelector('meta[name="theme-color"]');

  // If the meta tag doesn't exist, create it.
  if (!meta) {
    meta = document.createElement('meta');
    meta.setAttribute('name', 'theme-color');
    document.head.appendChild(meta);
  }

  // Set the content of the meta tag.
  meta.setAttribute('content', color);
}

// Default implementation, that you can customize
export default function Root({children}) {
  const { colorMode } = useColorMode();

  useEffect(() => {
    updateThemeColor(colorMode === 'dark' ? DARK_THEME_COLOR : LIGHT_THEME_COLOR);
  }, [colorMode]);

  return <>{children}</>;
}