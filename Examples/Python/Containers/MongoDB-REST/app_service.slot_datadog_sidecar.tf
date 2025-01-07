locals {
  telemetry_sidecar_env_file_content = file("${path.module}/.env sidecar")
}

resource "azurerm_linux_web_app_slot" "this_app_telemetry_sidecar" {
  name                = "${azurerm_linux_web_app.this_app_main.name}-telemetry-slot"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.this_app.name
  service_plan_id     = azurerm_service_plan.this_app.id
  parent_id           = azurerm_linux_web_app.this_app_main.id

  site_config {
    application_stack {
      docker_image_name        = "${var.acr_telemetry_sidecar_image_name}:${var.acr_telemetry_sidecar_tag}"
      docker_registry_url      = "https://${data.azurerm_container_registry.this_app.login_server}"
      docker_registry_username = data.azurerm_container_registry.this_app.admin_username
      docker_registry_password = data.azurerm_container_registry.this_app.admin_password
    }
  }

  app_settings = tomap({
    for line in split("\n", local.telemetry_sidecar_env_file_content) :
    split("=", line)[0] => split("=", line)[1]
    if line != "" && !startswith(line, "#")
  })
}