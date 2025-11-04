// API Response Models
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  errorMessage?: string;
  exception?: string;
}

// Project Models
export interface Project {
  id: number;
  name: string;
  description: string;
  rootPath: string;
}

export interface CreateProjectCommand {
  name: string;
  description: string;
  rootPath: string;
}

// File System Models
export interface FileSystemNode {
  id: number;
  projectId: number;
  parentId?: number;
  name: string;
  relativePath: string;
  isDirectory: boolean;
  size?: number;
  created: string;
  modified: string;
}

export interface FileSystemSearchParams {
  projectId: number;
  filter?: string;
  includeDirectories?: boolean;
  includeFiles?: boolean;
  pageNo?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNo: number;
  pageSize: number;
  totalPages: number;
}

// Object Hierarchy Models
export interface ObjectHierarchy {
  id: number;
  projectId: number;
  fileSystemNodeId: number;
  parentId?: number;
  identifierTypeId: number;
  identifierType: string;
  name: string;
  fullName: string;
  lineStart: number;
  lineEnd: number;
}

export interface ObjectHierarchySearchParams {
  projectId: number;
  searchTerm?: string;
  identifierTypeId?: number;
  fileSystemNodeId?: number;
  parentId?: number;
  pageNo?: number;
  pageSize?: number;
}

// Identifier Types
export enum IdentifierType {
  Namespace = 1,
  Interface = 2,
  Class = 3,
  Method = 4,
  Property = 5,
  Field = 6,
  Event = 7,
  MethodParam = 8
}

// Indexing Models
export interface IndexingQueueStatus {
  projectId?: number;
  queueSize: number;
  isProcessing: boolean;
  lastProcessed?: string;
}
