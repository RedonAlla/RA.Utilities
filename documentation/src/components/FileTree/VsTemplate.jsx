import React from 'react';
import TreeNode from './TreeNode';
import './styles.css';

// Data structure representing your file tree
const treeData = {
  name: 'RA.CleanArchitecture.Template',
  icon: 'folder',
  children: [
    { name: 'RA.CleanArchitecture.Template.sln', icon: 'sln' },
    { name: '.editorconfig', icon: 'editorconfig' },
    { name: 'Directory.Build.props', icon: 'xml' },
    { name: 'Directory.Build.targets', icon: 'xml' },
    { name: 'Directory.Packages.props', icon: 'xml' },
    {
      name: 'Core',
      icon: 'folder',
      children: [
        { name: 'RaTemplate.Domain', icon: 'csproj', children: [] },
        { name: 'RaTemplate.Application', icon: 'csproj', children: [] }
      ],
    },
    {
      name: 'Infrastructure',
      icon: 'folder',
      children: [
        { name: 'RaTemplate.Infrastructure', icon: 'csproj', children: [
          { name: '... (Application services, CQRS handlers, etc.)', icon: '', children: [] },
          { name: 'ApplicationServiceRegistration.cs', icon: 'csharp', children: [] },
        ] },
        { name: 'RaTemplate.Integration', icon: 'csproj', children: [] },
        { name: 'RaTemplate.Persistence', icon: 'csproj', children: [] }
      ]
    },
    {
      name: 'Web',
      icon: 'folder',
      children: [
        { name: 'RaTemplate.Api', icon: 'csproj', children: [] },
        { name: 'RaTemplate.Api.Contracts', icon: 'csproj', children: [] }
      ],
    }
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
