import React from 'react';
import TreeNode from './TreeNode';
import './styles.css';

// Data structure representing your file tree
const treeData = {
  name: 'RA.Utilities',
  icon: 'folder',
  children: [
    { name: 'RA.sln', icon: 'sln' },
    { name: '.editorconfig', icon: 'editorconfig' },
    { name: 'Directory.Build.props', icon: 'xml' },
    { name: 'Directory.Build.targets', icon: 'xml' },
    { name: 'Directory.Packages.props', icon: 'xml' },
    {
      name: 'Api',
      icon: 'folder',
      children: [
        { name: 'RA.Utilities.Api', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Api.Middlewares', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Api.Results', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Authentication.JwtBearer', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Authorization', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.OpenApi', icon: 'csproj', children: [] },
      ],
    },
    {
      name: 'Application',
      icon: 'folder',
      children: [{ name: 'RA.Utilities.Feature', icon: 'csproj', children: [] }],
    },
    {
      name: 'Core',
      icon: 'folder',
      children: [
        { name: 'RA.Utilities.Core', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Core.Constants', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Core.Exceptions', icon: 'csproj', children: [] },
      ],
    },
    {
      name: 'Data',
      icon: 'folder',
      children: [
        { name: 'RA.Utilities.Data.Abstractions', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Data.Entities', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Data.EntityFramework', icon: 'csproj', children: [] },
      ],
    },
    {
      name: 'Infrastructure',
      icon: 'folder',
      children: [{ name: 'RA.Utilities.Integrations', icon: 'csproj', children: [] }],
    },
    {
      name: 'Logging',
      icon: 'folder',
      children: [
        { name: 'RA.Utilities.Logging.Core', icon: 'csproj', children: [] },
        { name: 'RA.Utilities.Logging.Shared', icon: 'csproj', children: [] },
      ],
    },
    { name: 'documentation', icon: 'folder', children: [] },
  ],
};


const FileTree = () => {
  return (
    <div className="file-tree-container">
      <TreeNode node={treeData} />
    </div>
  );
};

export default FileTree;
