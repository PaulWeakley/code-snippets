locals {
  main_env_file_content = file("${path.module}/.env")
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

resource "azurerm_linux_web_app" "this_app_main" {
  name                = "${var.app_service_name}-${local.location_suffix}"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.this_app.name
  service_plan_id     = azurerm_service_plan.this_app.id

  site_config {
    application_stack {
      docker_image_name        = "${var.acr_image_name}:${var.acr_tag}"
      docker_registry_url      = "https://${data.azurerm_container_registry.this_app.login_server}"
      docker_registry_username = data.azurerm_container_registry.this_app.admin_username
      docker_registry_password = data.azurerm_container_registry.this_app.admin_password
    }
  }

  identity {
    type = "SystemAssigned"
  }

  app_settings = tomap({
    for line in split("\n", local.main_env_file_content) :
    split("=", line)[0] => split("=", line)[1]
    if line != "" && !startswith(line, "#")
  })

  dynamic "auth_settings" {
    for_each = []
    content {
      enabled = false
    }
  }
}