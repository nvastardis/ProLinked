import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';


import { AuthService } from './api/auth.service';
import { BlobService } from './api/blob.service';
import { ChatService } from './api/chat.service';
import { CommentService } from './api/comment.service';
import { ConnectionService } from './api/connection.service';
import { ConnectionRequestService } from './api/connectionRequest.service';
import { JobService } from './api/job.service';
import { ManageService } from './api/manage.service';
import { PostService } from './api/post.service';
import { ResumeService } from './api/resume.service';
import { SkillService } from './api/skill.service';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: [
    AuthService,
    BlobService,
    ChatService,
    CommentService,
    ConnectionService,
    ConnectionRequestService,
    JobService,
    ManageService,
    PostService,
    ResumeService,
    SkillService ]
})
export class ApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: ApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
