import React, { useState } from 'react';
import clsx from 'clsx';

const TreeNode = ({ node }) => {
  const { children, name, icon } = node;
  const [isOpen, setIsOpen] = useState(true);

  const isFolder = icon == 'folder';

  const handleToggle = () => {
    if (isFolder) {
      setIsOpen(!isOpen);
    }
  };

  // Determine the icon based on type and state
  const getIcon = () => {
    if (isFolder) {
      return isOpen ? 'folder-open' : 'folder-close';
    }
    
    return icon; 
  };

  return (
    <div className="tree-node">
      <div className={clsx('node-label', isFolder ? 'folder' : '')} onClick={handleToggle}>
        <span className={clsx('node-icon', 'folder', getIcon())}></span> {name}
      </div>
      {isFolder && isOpen && (
        <div className="node-children">
          {children.map((childNode, index) => (
            <TreeNode key={index} node={childNode} />
          ))}
        </div>
      )}
    </div>
  );
};

export default TreeNode;