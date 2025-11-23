import { TreeNode, TreeNodeAdapter } from '../models/tree-node.interface';
import { FileSystemNode } from '../models/api.models';

export class FileSystemTreeAdapter implements TreeNodeAdapter<FileSystemNode> {
  
  toTreeNode(item: FileSystemNode): TreeNode<FileSystemNode> {
    return {
      data: item,
      id: item.id,
      label: item.name,
      hasChildren: item.isDirectory,
      isExpanded: false,
      icon: this.getIcon(item)
    };
  }

  getId(item: FileSystemNode): number {
    return item.id;
  }

  getLabel(item: FileSystemNode): string {
    return item.name;
  }

  hasChildren(item: FileSystemNode): boolean {
    return item.isDirectory;
  }

  getIcon(item: FileSystemNode): string {
    if (item.isDirectory) return 'folder';
    
    // File type icons based on extension
    const ext = item.name.split('.').pop()?.toLowerCase();
    const iconMap: Record<string, string> = {
      'cs': 'csharp',
      'csproj': 'xml',
      'ts': 'typescript',
      'js': 'javascript',
      'json': 'json',
      'html': 'html',
      'css': 'css',
      'scss': 'scss',
      'md': 'markdown'      
    };
    return iconMap[ext || ''] || 'file';
  }
}