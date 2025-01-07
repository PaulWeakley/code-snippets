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