interface CloudEvent {
    specversion: string;
    type: string;
    source: string;
    subject: string;
    id: string;
    time: Date;
    datacontenttype: string;
    data: any;
}

class CloudEventImpl implements CloudEvent {
    specversion: string;
    type: string;
    source: string;
    subject: string;
    id: string;
    time: Date;
    datacontenttype: string;
    data: any;

    constructor(specversion: string, type: string, source: string, subject: string, datacontenttype: string, data: any) {
        this.specversion = specversion;
        this.type = type;
        this.source = source;
        this.subject = subject;
        this.id = this.generateUUID();
        this.time = new Date();
        this.datacontenttype = datacontenttype;
        this.data = data;
    }

    private generateUUID(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    toDict(): object {
        return {
            specversion: this.specversion,
            type: this.type,
            source: this.source,
            subject: this.subject,
            id: this.id,
            time: this.time.toISOString(),
            datacontenttype: this.datacontenttype,
            data: this.data
        };
    }
}