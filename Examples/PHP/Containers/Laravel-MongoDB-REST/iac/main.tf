locals {
    location_suffix = replace(lower(var.location), " ", "")
}

data "azurerm_resource_group" "this_app" {
  name = var.resource_group_name
}

data "azurerm_container_registry" "this_app" {
  name                = var.acr_name
  resource_group_name = data.azurerm_resource_group.this_app.name
}

resource "azurerm_service_plan" "this_app" {
  name                = "${var.app_service_name}-${local.location_suffix}-appserviceplan"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.this_app.name
  os_type             = "Linux"

  sku_name = var.plan_sku

  per_site_scaling_enabled = true
  zone_balancing_enabled   = true
}