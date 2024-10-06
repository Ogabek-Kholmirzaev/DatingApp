import { Component, inject } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent {
  member?: Member;
  private memberService = inject(MembersService);
  private accountService = inject(AccountService);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();

    if (!user) {
      return;
    }

    this.memberService.getMember(user.username).subscribe({
      next: member => this.member = member
    });
  }
}
