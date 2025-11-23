export interface TreeNode<T> {
  data: T;                      // The actual data (FileSystemNode, ObjectHierarchy, etc)
  id: string | number;          // Unique identifier
  label: string;                // Display name
  children?: TreeNode<T>[];     // Child nodes
  isExpanded?: boolean;         // Expansion state
  isLoading?: boolean;          // Loading state for lazy loading
  hasChildren?: boolean;        // Does this node have children?
  icon?: string;                // Optional icon name/class
  metadata?: Record<string, any>; // Any extra data you want
}

// Adapter interface - converts your domain models to TreeNode
export interface TreeNodeAdapter<T> {
  toTreeNode(item: T): TreeNode<T>;
  getId(item: T): string | number;
  getLabel(item: T): string;
  hasChildren(item: T): boolean;
  getIcon?(item: T): string;
}