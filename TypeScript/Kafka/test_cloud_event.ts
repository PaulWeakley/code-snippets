import { CloudEventImpl } from './cloud_event';

describe('CloudEventImpl', () => {
    let cloudEvent: CloudEventImpl;

    beforeEach(() => {
        cloudEvent = new CloudEventImpl(
            '1.0',
            'com.example.someevent',
            '/mycontext',
            '123',
            'application/json',
            { key: 'value' }
        );
    });

    test('should initialize properties correctly', () => {
        expect(cloudEvent.specversion).toBe('1.0');
        expect(cloudEvent.type).toBe('com.example.someevent');
        expect(cloudEvent.source).toBe('/mycontext');
        expect(cloudEvent.subject).toBe('123');
        expect(cloudEvent.id).toMatch(/^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i);
        expect(cloudEvent.time).toBeInstanceOf(Date);
        expect(cloudEvent.datacontenttype).toBe('application/json');
        expect(cloudEvent.data).toEqual({ key: 'value' });
    });

    test('should generate a valid UUID', () => {
        const uuid = cloudEvent['generateUUID']();
        expect(uuid).toMatch(/^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i);
    });

    test('should return correct dictionary representation', () => {
        const dict = cloudEvent.toDict();
        expect(dict).toEqual({
            specversion: '1.0',
            type: 'com.example.someevent',
            source: '/mycontext',
            subject: '123',
            id: expect.any(String),
            time: expect.any(String),
            datacontenttype: 'application/json',
            data: { key: 'value' }
        });
        expect(new Date(dict.time)).toBeInstanceOf(Date);
    });
});