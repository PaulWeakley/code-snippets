
class CloudEventHeaders(dict):
    @staticmethod
    def extract_default_headers(headers: dict=None) -> dict:
        specversion = headers.get('specversion', None) if headers else None
        type = headers.get('type', None) if headers else None
        source = headers.get('source', None) if headers else None
        subject = headers.get('subject', None) if headers else None
        return {
            'specversion': specversion, 
            'type': type,
            'source': source,
            'subject': subject
        }

    def __init__(self, specversion=None, type=None, source=None, subject=None):
        self['specversion'] = specversion
        self['type'] = type
        self['source'] = source
        self['subject'] = subject