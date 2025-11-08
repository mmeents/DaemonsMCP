// API Response Models - MATCH API CASING
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  errorMessage?: string; 
  exception?: string;
}

// Project Models - MATCH API CASING
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

// File System Models dto 
export interface FileSystemNode {
  id: number;
  projectId: number;
  parentId?: number;
  name: string;
  relativePath: string;
  isDirectory: boolean;  
  sizeInBytes?: number;  
}

export interface FileSystemSearchParams {
  projectId: number;
  filter?: string;
  includeDirectories?: boolean;
  includeFiles?: boolean;
  pageNo?: number;
  pageSize?: number;
}

// SINGLE PagedResult definition with correct casing
export interface PagedResult<T> {
  data: T[];           // Capital D - this is the file array
  totalCount: number;  // Capital T and C
  pageNo: number;      // Capital P and N
  pageSize: number;    // Capital P and S
  totalPages: number;  // Capital T and P
}

export interface FileSystemFile {
  projectId: number;
  fileSystemNodeId: number;
  sizeInBytes: number;
  relativePath: string;
  content: string;
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