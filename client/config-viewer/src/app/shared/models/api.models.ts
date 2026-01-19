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
  name:string;
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

// Item Models - add these to api.models.ts

export interface ItemDto {
  id: number;
  parentId?: number;
  itemTypeId: number;
  itemTypeName: string;
  statusTypeId: number;
  statusTypeName: string;
  rank: number;
  name: string;
  details: string;
  created: string;  // ISO date string
  modified: string; // ISO date string
  completed?: string; // ISO date string
  referenceFileSystemId?: number;
  referenceObjectHierarchyId?: number;
  children: ItemDto[];
}

export interface ItemTypeDto {
  id: number;
  name: string;
  description: string;
  rank: number;
  parentId?: number;
  isStatusType: boolean;
}

export interface AddUpdateItemRequest {
  id: number;
  parentId?: number;
  itemTypeId: number;
  statusTypeId: number;
  rank: number;
  name: string;
  details: string;
  referenceFileSystemId?: number;
  referenceObjectHierarchyId?: number;
}

export interface AddUpdateItemTypeRequest {
  id: number;
  name: string;
  description: string;
  rank: number;
  parentId?: number;
}

export interface SearchItemsParams {
  parentId?: number;
  nameContains?: string;
  detailsContains?: string;
  typeId?: number;
  statusId?: number;
  maxDepth?: number;
}

export enum DeleteStrategy {
  PreventIfHasChildren = 0,
  DeleteCascade = 1,
  OrphanChildren = 2,
  ReparentToGrandparent = 3
}

// User Models
export interface UserDto {
  id: number;
  email: string;
  displayName: string;
  hasPassword: boolean;
  hasGoogleAuth: boolean;
  hasGitHubAuth: boolean;
  createdAt: Date;
  lastLoginAt?: Date;
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  displayName: string;
  password: string;
}

// Access Token Models
export interface AccessTokenDto {
  id: number;
  parentId?: number;
  token: string;
  issuedTo: string;
  created: string;  // ISO date string
  expires: string;  // ISO date string
  used?: string;    // ISO date string
  usedUrl?: string;
  usedBy?: string;
  isValid: boolean;
  isExpired: boolean;
  usedUrlNextToken?: string;
}

export interface AccessTokenSearchParams {
  issuedTo?: string;
  includeExpired?: boolean;
  includeUsed?: boolean;
  pageNo?: number;
  pageSize?: number;
}

export interface CreateAccessTokenCommand {
  issuedTo: string;
  expiresInMinutes: number;
}

// Invitation Token Models
export interface InvitationTokenDto {
  id: number;
  token: string;
  invitedEmail?: string;
  createdByUserId: number;
  createdAt: string;  // ISO date string
  expiresAt: string;  // ISO date string
  isUsed: boolean;
  usedAt?: string;    // ISO date string
  usedByUserId?: number;
}

export interface InvitationTokenSearchParams {
  invitedEmail?: string;
  includeExpired?: boolean;
  includeUsed?: boolean;
  pageNo?: number;
  pageSize?: number;
}

export interface CreateInvitationCommand {
  createdByUserId: number;
  invitedEmail?: string;
  expiresInHours?: number; // Default 168 (7 days)
}

export interface RegisterWithInvitationCommand {
  invitationToken: string;
  email: string;
  displayName: string;
  password: string;
}

export interface GitRepositoryDto {
  id: number;
  projectId: number;
  localPath: string;
  remoteUrl: string;
  remoteName: string;
  currentBranchName: string;
  isDirty: boolean;
  modifiedFileCount?: number;
  untrackedFileCount?: number;
  lastFetchedAt?: string;  // ISO date string
  lastSyncedAt?: string;   // ISO date string
  createdAt: string;       // ISO date string
  updatedAt: string;       // ISO date string
}

export interface GitBranchDto {
  id: number;
  gitRepositoryId: number;
  name: string;
  isRemote: boolean;
  isHead: boolean;
  upstreamBranchName?: string;
  aheadBy?: number;
  behindBy?: number;
  lastCommitSha?: string;
  lastCommitMessage?: string;
  lastCommitAuthor?: string;
  lastCommitDate?: string;  // ISO date string
  createdAt: string;        // ISO date string
  updatedAt: string;        // ISO date string
}

// User Credentials Models
export interface UserCredentialDto {
  id: number;
  userId: number;
  name: string;
  providerType: ProviderType;
  credentialType: CredentialType;
  hasUsername: boolean;  // Security: Never expose encrypted values
  hasSecret: boolean;    // Just indicate if set
  createdDate: string;   // ISO date string
  lastUsedDate?: string; // ISO date string
  isActive: boolean;
}

export interface CreateUserCredentialCommand {
  userId: number;
  name: string;
  providerType: ProviderType;
  credentialType: CredentialType;
  username: string;
  secret: string;  // PAT, password, etc.
}

export interface UpdateUserCredentialCommand {
  id: number;
  name?: string;
  username?: string;  // Null = don't update
  secret?: string;    // Null = don't update
  isActive?: boolean; // Null = don't update
}

export enum ProviderType {
  GitHub = 1,
  AzureDevOps = 2,
  Bitbucket = 3,
  GitLab = 4
}

export enum CredentialType {
  PersonalAccessToken = 1,
  UsernamePassword = 2,
  SshKey = 3
}

export interface ModelDto {
  id: number;
  projectId: number;
  parentId?: number;
  modelTypeId: number;
  modelTypeName: string;
  name: string;
  rank: number;
  code?: string;
  createdDate: string;  // ISO date string
  modifiedDate: string; // ISO date string
  properties: ModelPropertyDto[];
  children: ModelDto[];
}

export interface ModelPropertyDto {
  id: number;
  modelId: number;
  propertyKey: string;
  propertyValue?: string;
  propertyValueTypeId?: number;
  propertyValueTypeName?: string;
  propertyEditorTypeId?: number;
  propertyEditorTypeName?: string;
}

export interface ModelTypeDto {
  id: number;
  ownerTypeId?: number;
  categoryTypeId?: number;
  editorTypeId?: number;
  typeRank: number;
  name: string;
  description: string;
  isVisible: boolean;
  isReadonly: boolean;
  iconName: string;
  children: ModelTypeDto[];
}

export interface AddUpdateModelRequest {
  id: number;
  projectId: number;
  parentId?: number;
  modelTypeId: number;
  name: string;
  rank: number;
  code?: string;
}

export interface AddUpdateModelPropertyRequest {
  id: number;
  modelId: number;
  propertyKey: string;
  propertyValue?: string;
  propertyValueTypeId?: number;
  propertyEditorTypeId?: number;
}

export interface SearchModelsParams {
  projectId: number;
  parentId?: number;
  modelTypeId?: number;
  nameFilter?: string;
  maxDepth?: number;
  includeProperties?: boolean;
}

export interface ExecuteTemplateCommand {
  templateId: number;
  targetModelId?: number;
  saveToFile?: boolean;
}

export interface TemplateExecutionResult {
  success: boolean;
  renderedCode: string;
  relativeFileName: string;
  errorMessage?: string;
  generatedFiles: string[];
}