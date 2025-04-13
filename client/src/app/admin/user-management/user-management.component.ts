import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModelComponent } from '../../modals/roles-model/roles-model.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private modelService = inject(BsModalService);
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModelComponent> = new BsModalRef<RolesModelComponent>();

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserRoles().subscribe({
      next: (users) => this.users = users
    });
  }

  openRolesModal(user: User) {
    const initialState: ModalOptions = {
      class: "modal-lg",
      initialState: {
        title: 'User roles',
        username: user.username,
        availableRoles: ['Admin', 'Moderator', 'Member'],
        selectedRoles: [...user.roles],
        users: this.users,
        rolesUpdated: false
      }
    }

    this.bsModalRef = this.modelService.show(RolesModelComponent, initialState);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated) {
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService.updateUserRoles(user.username, selectedRoles).subscribe({
            next: roles => user.roles = roles
          })
        }
      }
    })
  }
}
