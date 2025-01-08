variable "plan_sku" {
  type        = string
  description = "The sku of app service plan to create"
  default     = "P0v3"
}

variable "location" {
    description = "The Azure region to deploy resources."
    type        = string
    default     = "East US2"
}

variable "resource_group_name" {
    description = "Name of the resource group."
    type        = string
    default     = "mongodbtest"
}

variable "acr_name" {
    description = "Name of the Azure Container Registry."
    type        = string
    default     = "nodejstypescriptmongodbrestapi"
}

variable "acr_image_name" {
    description = "Name of the ACR image."
    type        = string
    default     = "nodejs-typescript-mongodb-rest-api"
}

variable "acr_tag" {
    description = "Tag for the ACR image."
    type        = string
    default     = "latest"
}

variable "acr_telemetry_sidecar_image_name" {
    description = "Name of the ACR image."
    type        = string
    default     = "datadoghq/agent"
}

variable "acr_telemetry_sidecar_tag" {
    description = "Tag for the ACR image."
    type        = string
    default     = "latest"
}

variable "app_service_name" {
    description = "Name of the App Service."
    type        = string
    default     = "nodejs-typescript-mongodb-rest-api"
}

variable "subscription_id" {
    description = "The Azure subscription ID."
    type        = string
    default     = "58853283-7f1b-40e1-b0c4-aa0c95d51f63"
}