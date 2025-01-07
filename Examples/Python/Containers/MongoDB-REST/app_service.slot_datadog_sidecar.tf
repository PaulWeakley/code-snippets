locals {
  telemetry_sidecar_env_file_content = file("${path.module}/.env-sidecar")
}

resource "azurerm_linux_web_app_slot" "this_app_telemetry_sidecar" {
  name                = "${azurerm_linux_web_app.this_app_main.name}-telemetry-slot"
  app_service_id      = azurerm_linux_web_app.this_app_main.id

  site_config {
    application_stack {
      docker_image_name        = "${var.acr_telemetry_sidecar_image_name}:${var.acr_telemetry_sidecar_tag}"
      docker_registry_url      = "https://${data.azurerm_container_registry.this_app.login_server}"
      docker_registry_username = data.azurerm_container_registry.this_app.admin_username
      docker_registry_password = data.azurerm_container_registry.this_app.admin_password
    }
  }

  app_settings = {
    SLOT_TYPE = "Sidecar"
  }
}